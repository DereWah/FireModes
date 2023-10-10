using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using FireModes.Types;
using FireModes.Utils;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireModes.EventHandlers
{
    public class PlayerHandler
    {

        private readonly Main plugin;
        private readonly Config config;
        private readonly Utilities utilities;

        public PlayerHandler(Main plugin)
        {
            this.plugin = plugin;
            config = plugin.Config;
            utilities = plugin.Utilities;
        }

        public void ReloadingWeapon(ReloadingWeaponEventArgs e)
        {
            if (!config.FiremodeWeapons.ContainsKey(e.Firearm.Type)) return;

            utilities.RegisterWeapon(e.Firearm);
            //Log.Info($"Reloading Allowed: {CanReload(e.Player, e.Firearm)}");
            WeaponData wd = plugin.WeaponMemory[e.Firearm.Serial];
            if (!wd.CanReload(e.Player))
            {
                e.IsAllowed = false;
            }
            else
            {
                //we store the weapon virtual ammo in the actual weapon
                //so that the reloading process uses the correct amount of ammo
                e.Firearm.Ammo = wd.CurrentAmmo;
                //Log.Info("Before: " + e.Player.IsReloading);

                Timing.CallDelayed(2f, () =>
                {
                    Timing.RunCoroutine(utilities.FinishReloading(e.Player, e.Firearm, wd));
                });
            } 
        }

        public void UnloadingWeapon(UnloadingWeaponEventArgs e)
        {
            if (config.FiremodeWeapons.ContainsKey(e.Firearm.Type))
            {
                utilities.RegisterWeapon(e.Firearm);
                WeaponData wd = plugin.WeaponMemory[e.Firearm.Serial];
                //to allow the unloading to work, we set the weapon amount to 1
                //then we give to the player the ammo from the virtual mag - 1
                //and we set the virtual ammo to 0
                e.Firearm.Ammo = 1;
                e.Player.AddAmmo(e.Firearm.AmmoType, (byte)(wd.CurrentAmmo - 1));
                wd.CurrentAmmo = 0;
            }

        }

        public void Shooting(ShootingEventArgs e)
        {
            if (!config.FiremodeWeapons.Keys.Contains(e.Firearm.Type))
            {
                return;
            }

            utilities.RegisterWeapon(e.Firearm);
            WeaponData wd = plugin.WeaponMemory[e.Firearm.Serial];
            //in this case we prevent player from shooting (they're out of ammo)
            //and sync the firearm mag with the virtual

            switch (wd.CurrentAmmo)
            {
                case <= 0:
                    e.IsAllowed = false;
                    e.Firearm.Ammo = 0;
                    break;
                case > 0:
                    //we remove the ammo we just shot from the digital mag
                    wd.CurrentAmmo -= 1;
                    if (e.Firearm.Ammo == 0)
                    {
                        //if the gun is empty we update the weapon
                        //the gun empty doesn't mean the virtual mag is empty!
                        wd.UpdateWeapon();
                    }
                    break;
            }
        }

        public void TogglingNoClip(TogglingNoClipEventArgs e)
        {
            //a player can toggle the fire mode only if:
            //1. his item is set
            //2. his item is part of the weapon config
            //3. he is not reloading / unloading (done to prevent de-sync)

            if(!(e.Player.CurrentItem == null
                || !config.FiremodeWeapons.ContainsKey(e.Player.CurrentItem.Type)
                || e.Player.IsReloading)
                && e.Player.CurrentItem.IsWeapon)
            { 
                e.IsAllowed = false; //prevent from going in noclip
                utilities.RegisterWeapon((Firearm) e.Player.CurrentItem);
                WeaponData wd = plugin.WeaponMemory[e.Player.CurrentItem.Serial];
                wd.ScrollFireMode();

                if (config.ShowMessageOnFiremodeChange)
                {
                    string hint = wd.BuildHint();
                    e.Player.ShowHint(hint);
                }

            }
        }


    }
}

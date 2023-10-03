using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using FireModes.Types;
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

        public IEnumerator<float> FinishReloading(Player p, Firearm firearm, WeaponData wd)
        {
            Main Plugin = Main.Singleton;

            yield return Timing.WaitUntilFalse(() => p.IsReloading);

            //after reloading is done we update the virtual mag ammo with the new amount
            wd.CurrentAmmo = firearm.Ammo;

            //then we update the weapon to match its fire mode
            Plugin.Utilities.UpdateWeapon(firearm);
        }

        public void ReloadingWeapon(ReloadingWeaponEventArgs e)
        {

            Main Plugin = Main.Singleton;
            Config Config = Plugin.Config;


            if (!Config.FiremodeWeapons.ContainsKey(e.Firearm.Type)) return;
            Plugin.Utilities.RegisterWeapon(e.Firearm);
            //Log.Info($"Reloading Allowed: {CanReload(e.Player, e.Firearm)}");
            WeaponData wd = Plugin.WeaponMemory[e.Firearm.Serial];
            if (!Plugin.Utilities.CanReload(e.Player, e.Firearm)) e.IsAllowed = false;
            else
            {
                //we store the weapon virtual ammo in the actual weapon
                //so that the reloading process uses the correct amount of ammo
                e.Firearm.Ammo = wd.CurrentAmmo;
                //Log.Info("Before: " + e.Player.IsReloading);

                Timing.CallDelayed(2f, () =>
                {

                    Timing.RunCoroutine(FinishReloading(e.Player, e.Firearm, wd));
                });
            } 
        }

        public void UnloadingWeapon(UnloadingWeaponEventArgs e)
        {
            Main Plugin = Main.Singleton;
            Config Config = Plugin.Config;

            if (!e.IsAllowed) return;
            if (!Config.FiremodeWeapons.ContainsKey(e.Firearm.Type)) return;
            Plugin.Utilities.RegisterWeapon(e.Firearm);
            WeaponData wd = Plugin.WeaponMemory[e.Firearm.Serial];
            //to allow the unloading to work, we set the weapon amount to 1
            //then we give to the player the ammo from the virtual mag - 1
            //and we set the virtual ammo to 0
            e.Firearm.Ammo = 1;
            e.Player.AddAmmo(e.Firearm.AmmoType, (byte) (wd.CurrentAmmo-1));
            wd.CurrentAmmo = 0;
        }

        public void Shooting(ShootingEventArgs e)
        {
            Main Plugin = Main.Singleton;
            Config Config = Plugin.Config;

            if (!Config.FiremodeWeapons.Keys.Contains(e.Firearm.Type)) return;

            Plugin.Utilities.RegisterWeapon(e.Firearm);
            WeaponData wd = Plugin.WeaponMemory[e.Firearm.Serial];
            //in this case we prevent player from shooting (they're out of ammo)
            //and sync the firearm mag with the virtual
            
            if(wd.CurrentAmmo <= 0)
            {
                e.IsAllowed = false;
                e.Firearm.Ammo = 0;
            } else
            {
               
                //we remove the ammo we just shot from the digital mag
                wd.CurrentAmmo -= 1;
                if(e.Firearm.Ammo == 0)
                {
                    //if the gun is empty we update the weapon
                    //the gun empty doesn't mean the virtual mag is empty!
                    Plugin.Utilities.UpdateWeapon(e.Firearm);
                }
            }
        }

        public void TogglingNoClip(TogglingNoClipEventArgs e)
        {
            Main Plugin = Main.Singleton;
            Config Config = Plugin.Config;


            //a player can toggle the fire mode only if:
            //1. his item is set
            //2. his item is part of the weapon config
            //3. he is not reloading / unloading (done to prevent de-sync)
            if (e.Player.CurrentItem == null ||
                !Config.FiremodeWeapons.ContainsKey(e.Player.CurrentItem.Type) ||
                e.Player.IsReloading) return;
            if (e.Player.CurrentItem.IsWeapon)
            {

                e.IsAllowed = false; //prevent from going in noclip
                Plugin.Utilities.RegisterWeapon((Firearm) e.Player.CurrentItem);
                WeaponData wd = Plugin.WeaponMemory[e.Player.CurrentItem.Serial];
                wd.FireMode = Plugin.Utilities.GetNextFireMode(e.Player.CurrentItem.Type, wd.FireMode);
                if (Config.ShowMessageOnFiremodeChange)
                {
                    string Hint = Plugin.Utilities.BuildHint(wd.FireMode, wd.CurrentAmmo, ((Firearm)e.Player.CurrentItem).MaxAmmo);
                    e.Player.ShowHint(Hint);
                }
                Plugin.Utilities.UpdateWeapon((Firearm)e.Player.CurrentItem);
                
            }
        }


    }
}

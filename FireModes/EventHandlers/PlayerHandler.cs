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

        public void RegisterWeapon(Firearm firearm)
        {
            //register the weapon to the WeaponMemory

            if (!Main.Singleton.Config.FireModeWeapons.Contains(firearm.Type)) return;

            if (!Main.Singleton.WeaponMemory.ContainsKey(firearm.Serial))
            {
                //WeaponData is a class that tracks the weapons' mag
                WeaponData wd = new WeaponData()
                {
                    CurrentAmmo = firearm.Ammo,
                    FireMode = Enums.FiringModes.Auto
                };
                Main.Singleton.WeaponMemory.Add(firearm.Serial, wd);
            }
        }

        public bool CanReload(Player player, Firearm firearm)
        {
            
            WeaponData wd = Main.Singleton.WeaponMemory[firearm.Serial];
            //player can't reload if:
            //has 0 ammo or his gun is full ammo (in case the weapon is set in auto mode)
            //or his gun virtual mag is full ammo (in case weapon is set in single or burst)
            if (player.GetAmmo(firearm.AmmoType) == 0) return false;
            if (firearm.Ammo == firearm.MaxAmmo) return false;
            if (wd.CurrentAmmo == firearm.MaxAmmo) return false;
            return true;
        }



        public void ReloadingWeapon(ReloadingWeaponEventArgs e)
        {

            if (!Main.Singleton.Config.FireModeWeapons.Contains(e.Firearm.Type)) return;
            RegisterWeapon(e.Firearm);
            //Log.Info($"Reloading Allowed: {CanReload(e.Player, e.Firearm)}");
            WeaponData wd = Main.Singleton.WeaponMemory[e.Firearm.Serial];
            if (!CanReload(e.Player, e.Firearm)) e.IsAllowed = false;
            else
            {
                //we store the weapon virtual ammo in the actual weapon
                //so that the reloading process uses the correct amount of ammo
                e.Firearm.Ammo = wd.CurrentAmmo;
                Timing.CallDelayed(2f, () =>
                {
                    //after reloading is done we update the virtual mag ammo with the new amount
                    wd.CurrentAmmo = e.Firearm.Ammo;
                    //Log.Info($"Updated ammos: {wd.CurrentAmmo}");
                    //then we update the weapon to match its fire mode
                    UpdateWeapon(e.Firearm);
                });
            } 
        }

        public void UnloadingWeapon(UnloadingWeaponEventArgs e)
        {
            if (!e.IsAllowed) return;
            if (!Main.Singleton.Config.FireModeWeapons.Contains(e.Firearm.Type)) return; 
            RegisterWeapon(e.Firearm);
            WeaponData wd = Main.Singleton.WeaponMemory[e.Firearm.Serial];
            //to allow the unloading to work, we set the weapon amount to 1
            //then we give to the player the ammo from the virtual mag - 1
            //and we set the virtual ammo to 0
            e.Firearm.Ammo = 1;
            e.Player.AddAmmo(e.Firearm.AmmoType, (byte) (wd.CurrentAmmo-1));
            wd.CurrentAmmo = 0;
        }

        public void Shooting(ShootingEventArgs e)
        {
            if (!Main.Singleton.Config.FireModeWeapons.Contains(e.Firearm.Type)) return;
            
            RegisterWeapon(e.Firearm);
            WeaponData wd = Main.Singleton.WeaponMemory[e.Firearm.Serial];

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
                    UpdateWeapon(e.Firearm);
                }
            }
        }

        public void UpdateWeapon(Firearm firearm)
        {
            //this function is made to sync the firearm mag with the virtual mag

            WeaponData wd = Main.Singleton.WeaponMemory[firearm.Serial];
            //Log.Info($"Reamining Ammo: {wd.CurrentAmmo}");
            //if the weapon virtual mag has 0 ammo we don't bother changing the firearm
            if (wd.CurrentAmmo <= 0) return;
            switch (wd.FireMode)
            {
                case Enums.FiringModes.Burst:
                    //if the player has more than enough ammo we set to default 3
                    //otherwise we set the ammo to what remains of his virtual mag
                    if (wd.CurrentAmmo >= 3) firearm.Ammo = 3;
                    else firearm.Ammo = wd.CurrentAmmo;
                    break;
                case Enums.FiringModes.Single:
                    if(wd.CurrentAmmo > 0) firearm.Ammo = 1;
                    break;
                case Enums.FiringModes.Auto:
                    //all of the virtual mag ammos are loaded in the firearm
                    firearm.Ammo = wd.CurrentAmmo;
                    break;
            }
        }

        public void TogglingNoClip(TogglingNoClipEventArgs e)
        {
            //a player can toggle the fire mode only if:
            //1. his item is set
            //2. his item is part of the weapon config
            //3. he is not reloading / unloading (done to prevent de-sync)
            if (e.Player.CurrentItem == null) return;
            if (!Main.Singleton.Config.FireModeWeapons.Contains(e.Player.CurrentItem.Type)) return;
            if (e.Player.IsReloading) return;
            if (e.Player.CurrentItem.IsWeapon)
            {
                RegisterWeapon((Firearm) e.Player.CurrentItem);
                WeaponData wd = Main.Singleton.WeaponMemory[e.Player.CurrentItem.Serial];
                wd.FireMode += 1;
                if ((int) wd.FireMode > 2) wd.FireMode = 0;
                e.Player.ShowHint($"<align=left>Firing Mode: {wd.FireMode}</align>\n<align=right>Ammo: {wd.CurrentAmmo} / {((Firearm) e.Player.CurrentItem).MaxAmmo}</align>");
                UpdateWeapon((Firearm)e.Player.CurrentItem);
                
            }
        }
    }
}

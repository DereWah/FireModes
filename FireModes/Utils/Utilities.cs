using Exiled.API.Features;
using Exiled.API.Features.Items;
using FireModes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireModes.Utils
{
    public class Utilities
    {
        public void RegisterWeapon(Firearm firearm)
        {

            Main Plugin = Main.Singleton;
            Config Config = Plugin.Config;

            //register the weapon to the WeaponMemory
            if (!Config.FiremodeWeapons.ContainsKey(firearm.Type)) return;
            if (!Plugin.WeaponMemory.ContainsKey(firearm.Serial))
            {
                //Log.Info("registering weapon...");
                //WeaponData is a class that tracks the weapons' mag
                WeaponData wd = new WeaponData()
                {
                    CurrentAmmo = firearm.Ammo,
                    FireMode = FiringModes.Auto,
                    DefaultFireRate = firearm.FireRate
                };
                Plugin.WeaponMemory.Add(firearm.Serial, wd);
            }
        }


        public string BuildHint(FiringModes FiringMode, byte CurrentAmmo, byte MaxAmmo)
        {
            Config Config = Main.Singleton.Config;


            string UnformattedString = Config.FiremodeChangeMessage;
            if (!Config.FiremodeTranslation.TryGetValue(FiringMode, out string FiringmodeTranslation))
                FiringmodeTranslation = $"{FiringMode}";
            return UnformattedString.Replace("{ammo}", $"{CurrentAmmo}").Replace("{maxammo}", $"{MaxAmmo}").Replace("{firemode}", $"{FiringmodeTranslation}");
        }

        public FiringModes GetNextFireMode(ItemType item, FiringModes firemode)
        {
            Config Config = Main.Singleton.Config;

            List<FiringModes> FModes = Config.FiremodeWeapons[item];
            int index = FModes.IndexOf(firemode);

            if (index == -1)
            {
                // Handle the case where the current firing mode is not found in the list.
                // You can throw an exception, return a default value, or handle it as needed.
                // For now, let's return the first firing mode in the list.
                return FModes[0];
            }

            index = (index + 1) % FModes.Count;
            return FModes[index];
        }

        public void UpdateWeapon(Firearm firearm)
        {
            //this function is made to sync the firearm mag with the virtual mag
            Main Plugin = Main.Singleton;
            Config Config = Plugin.Config;

            WeaponData wd = Plugin.WeaponMemory[firearm.Serial];
            //Log.Info($"Reamining Ammo: {wd.CurrentAmmo}");
            //if the weapon virtual mag has 0 ammo we don't bother changing the firearm
            if (wd.CurrentAmmo <= 0) return;
            switch (wd.FireMode)
            {
                case FiringModes.Burst:
                    if (firearm.FireRate == wd.DefaultFireRate)
                        firearm.FireRate *= Config.BurstRateMultiplier;
                    //if the player has more than enough ammo we set to default 3
                    //otherwise we set the ammo to what remains of his virtual mag
                    if (wd.CurrentAmmo >= 3) firearm.Ammo = 3;
                    else firearm.Ammo = wd.CurrentAmmo;
                    break;
                case FiringModes.Single:
                    firearm.FireRate = wd.DefaultFireRate;
                    if (wd.CurrentAmmo > 0) firearm.Ammo = 1;
                    break;
                case FiringModes.Auto:
                    firearm.FireRate = wd.DefaultFireRate;
                    //all of the virtual mag ammos are loaded in the firearm
                    firearm.Ammo = wd.CurrentAmmo;
                    break;
            }
        }

        public bool CanReload(Player player, Firearm firearm)
        {
            Main Plugin = Main.Singleton;

            WeaponData wd = Plugin.WeaponMemory[firearm.Serial];
            //player can't reload if:
            //has 0 ammo or his gun is full ammo (in case the weapon is set in auto mode)
            //or his gun virtual mag is full ammo (in case weapon is set in single or burst)
            if (player.GetAmmo(firearm.AmmoType) == 0 || 
                firearm.Ammo == firearm.MaxAmmo ||
                wd.CurrentAmmo == firearm.MaxAmmo) return false;
            return true;
        }
    }
}

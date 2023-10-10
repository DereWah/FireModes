using Exiled.API.Features;
using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireModes.Types
{
    public class WeaponData
    {
        public Firearm Weapon;
        public byte CurrentAmmo;
        public FiringModes FireMode;
        public float DefaultFireRate;

        /// <summary>
        /// Constructs a new weapon data for a weapon.
        /// </summary>
        /// <param name="weapon">The weapon which this weapon data will refer to.</param>
        public WeaponData(Firearm weapon)
        {
            Weapon = weapon;
            CurrentAmmo = weapon.Ammo;
            FireMode = FiringModes.Auto;
            DefaultFireRate = weapon.FireRate;
        }

        /// <summary>
        /// Returns wether this weapon can be reloaded by the player.
        /// </summary>
        /// <param name="player">The player to check if they can reload.</param>
        /// <returns></returns>
        public bool CanReload(Player player)
        {
            //player can't reload if:
            //has 0 ammo or his gun is full ammo (in case the weapon is set in auto mode)
            //or his gun virtual mag is full ammo (in case weapon is set in single or burst)
            if (player.GetAmmo(Weapon.AmmoType) == 0 ||
                Weapon.Ammo == Weapon.MaxAmmo ||
                CurrentAmmo == Weapon.MaxAmmo)
            {
                return false;
            }
 
            return true;
        }


        /// <summary>
        /// Creates a string to be put as a Hint on the screen.
        /// </summary>
        /// <returns>The string to put in the hint</returns>
        public string BuildHint()
        {
            Config config = Main.Instance.Config;

            string UnformattedString = config.FiremodeChangeMessage;
            if (!config.FiremodeTranslation.TryGetValue(FireMode, out string FiringmodeTranslation))
            {
                FiringmodeTranslation = $"{FireMode}";
            }

            return UnformattedString.Replace("{ammo}", $"{CurrentAmmo}").Replace("{maxammo}", $"{Weapon.MaxAmmo}").Replace("{firemode}", $"{FiringmodeTranslation}");
        }

        /// <summary>
        /// Switches the firearm to the next available fire mode.
        /// </summary>
        /// <returns>The new chosen firemode.</returns>
        public FiringModes ScrollFireMode()
        {
            Config config = Main.Instance.Config;

            List<FiringModes> fModes = config.FiremodeWeapons[Weapon.Type];
            int index = fModes.IndexOf(FireMode);

            if (index == -1)
            {
                // Handle the case where the current firing mode is not found in the list.
                // You can throw an exception, return a default value, or handle it as needed.
                // For now, let's return the first firing mode in the list.
                return fModes[0];
            }

            index = (index + 1) % fModes.Count;
            FireMode = fModes[index];
            UpdateWeapon();
            return FireMode;
        }

        /// <summary>
        /// This method syncs the firearm mag with its virtual mag.
        /// </summary>
        public void UpdateWeapon()
        {
            Main plugin = Main.Instance;
            Config config = plugin.Config;
            //this function is made to sync the firearm mag with the virtual mag

            //Log.Info($"Reamining Ammo: {wd.CurrentAmmo}");
            //if the weapon virtual mag has 0 ammo we don't bother changing the firearm
            if (CurrentAmmo <= 0) return;
            switch (FireMode)
            {
                case FiringModes.Burst when CurrentAmmo >= 3:
                    //if the player has more than enough ammo we set to default 3
                    Weapon.Ammo = 3;
                    goto case FiringModes.Burst;
                case FiringModes.Burst when CurrentAmmo < 3:
                    //otherwise we set the ammo to what remains of his virtual mag
                    Weapon.Ammo = CurrentAmmo;
                    goto case FiringModes.Burst;
                case FiringModes.Burst:
                    if (Weapon.FireRate == DefaultFireRate)
                    {
                        Weapon.FireRate *= config.BurstRateMultiplier;
                    }
                    break;
                case FiringModes.Single when CurrentAmmo > 0:
                    Weapon.FireRate = DefaultFireRate;
                    Weapon.Ammo = 1;
                    break;
                case FiringModes.Auto:
                    Weapon.FireRate = DefaultFireRate;
                    //all of the virtual mag ammos are loaded in the firearm
                    Weapon.Ammo = CurrentAmmo;
                    break;
            }
        }

    }
}

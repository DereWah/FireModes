using Exiled.API.Features;
using Exiled.API.Features.Items;
using FireModes.Types;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireModes.Utils
{
    public class Utilities
    {
        private readonly Main plugin;
        private readonly Config config;
        private readonly Utilities utilities;

        public Utilities(Main plugin)
        {
            this.plugin = plugin;
            config = plugin.Config;
            utilities = plugin.Utilities;
        }

        public WeaponData RegisterWeapon(Firearm firearm)
        {
            //register the weapon to the WeaponMemory
            if (config.FiremodeWeapons.ContainsKey(firearm.Type)
               && !plugin.WeaponMemory.ContainsKey(firearm.Serial))
            {
                //Log.Info("registering weapon...");
                //WeaponData is a class that tracks the weapons' mag
                WeaponData wd = new WeaponData(firearm);
                plugin.WeaponMemory.Add(firearm.Serial, wd);
                return wd;
            }
            return null;
        }

        public IEnumerator<float> FinishReloading(Player p, Firearm firearm, WeaponData wd)
        {
            yield return Timing.WaitUntilFalse(() => p.IsReloading);

            //after reloading is done we update the virtual mag ammo with the new amount
            wd.CurrentAmmo = firearm.Ammo;

            //then we update the weapon to match its fire mode
            wd.UpdateWeapon();
        }
    }
}

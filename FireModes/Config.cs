using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FireModes.Types.Enums;

namespace FireModes
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("List of weapons that will work with FireModes. Not automatic weapons will not work.")]
        public Dictionary<ItemType, List<FiringModes>> FiremodeWeapons { get; set; } = new Dictionary<ItemType, List<FiringModes>>()
        {
            {
                ItemType.GunE11SR, new List<FiringModes>()
                {
                    FiringModes.Single,
                    FiringModes.Auto,
                    FiringModes.Burst
                }
            },
            {
                ItemType.GunLogicer, new List<FiringModes>()
                {
                    FiringModes.Single,
                    FiringModes.Auto,
                    FiringModes.Burst
                }
            },
            {
                ItemType.GunFSP9, new List<FiringModes>()
                {
                    FiringModes.Single,
                    FiringModes.Auto,
                    FiringModes.Burst
                }
            },
            {
                ItemType.GunCrossvec, new List<FiringModes>()
                {
                    FiringModes.Single,
                    FiringModes.Auto,
                    FiringModes.Burst
                }
            },
            {
                ItemType.GunFRMG0, new List<FiringModes>()
                {
                    FiringModes.Single,
                    FiringModes.Auto,
                    FiringModes.Burst
                }
            },
            {
                ItemType.GunAK, new List<FiringModes>()
                {
                    FiringModes.Single,
                    FiringModes.Auto,
                    FiringModes.Burst
                }
            }
        };

        [Description("How much faster will a gun shoot when in burst mode.")]
        public float BurstRateMultiplier { get; set; } = 2f;

        public bool ShowMessageOnFiremodeChange { get; set; } = true;
        [Description("Available placeholders: {firemode}, {ammo}, {maxammo}")]
        public string FiremodeChangeMessage { get; set; } = "<align=left>Firing Mode: {firemode}\\n<align=right>Ammo: {ammo}/{maxammo}";
        public Dictionary<FiringModes, string> FiremodeTranslation { get; set; } = new Dictionary<FiringModes, string>()
        {
            { FiringModes.Single, "<color=blue>Single</color>" },
            { FiringModes.Auto, "<color=green>Auto</color>" },
            { FiringModes.Burst, "<color=red>Burst</color>" }
        };

    }
}

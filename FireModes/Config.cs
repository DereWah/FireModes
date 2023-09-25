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
        [Description("List of weapons that will work with FireModes")]
        public Dictionary<ItemType, List<FiringModes>> FiremodeWeapons { get; set; } = new Dictionary<ItemType, List<FiringModes>>()
        {
            {
                ItemType.GunE11SR, new List<FiringModes>()
                {
                    FiringModes.Single,
                    FiringModes.Auto,
                    FiringModes.Burst
                }
            }
        };

        [Description("How much faster will a gun shoot when in burst mode.")]
        public float BurstRateMultiplier { get; set; } = 2f;

    }
}

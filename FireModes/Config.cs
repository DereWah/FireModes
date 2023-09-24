using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireModes
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("List of weapons that will work with FireModes")]
        public List<ItemType> FireModeWeapons { get; set; } = new List<ItemType>()
        {
            ItemType.GunE11SR,
            ItemType.GunAK,
            ItemType.GunCrossvec,
            ItemType.GunFSP9,
            ItemType.GunFRMG0
        };

    }
}

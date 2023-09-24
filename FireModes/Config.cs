using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireModes
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public List<ItemType> FireModeWeapons { get; set; } = new List<ItemType>()
        {
            ItemType.GunE11SR,
            ItemType.GunAK
        };

    }
}

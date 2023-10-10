using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireModes.EventHandlers
{
    class ServerHandler
    {
        private readonly Main plugin;

        public ServerHandler(Main plugin) => this.plugin = plugin;


        public void WaitingForPlayers()
        {
            plugin.WeaponMemory.Clear();
        }
    }
}

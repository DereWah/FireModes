using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireModes.EventHandlers
{
    class ServerHandler
    {

        public void WaitingForPlayers()
        {
            Main.Singleton.WeaponMemory.Clear();
        }
    }
}

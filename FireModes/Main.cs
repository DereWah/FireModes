using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.Generic;
using Exiled.Events;
using FireModes.Types;
using FireModes.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace FireModes
{
    public class Main : Plugin<Config>
    {

        public override string Author => "@derewah";
        public override string Name => "FireModes";
        public override string Prefix => "FireModes";
        public override PluginPriority Priority => PluginPriority.Medium;

        private EventHandlers.PlayerHandler PlayerHandler;

        private EventHandlers.ServerHandler ServerHandler;

        public static Main Instance;

        public Dictionary<ushort, WeaponData> WeaponMemory = new Dictionary<ushort, WeaponData>();

        public Utilities Utilities;

        public override void OnEnabled()
        {
            base.OnEnabled();
            Instance = this;
            Utilities = new Utilities(this);
            RegisterEvents();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            base.OnDisabled();
        }

        public void RegisterEvents()
        {
            ServerHandler = new EventHandlers.ServerHandler(this);
            PlayerHandler = new EventHandlers.PlayerHandler(this);

            Player.ReloadingWeapon += PlayerHandler.ReloadingWeapon;
            Player.Shooting += PlayerHandler.Shooting;
            Player.UnloadingWeapon += PlayerHandler.UnloadingWeapon;
            Player.TogglingNoClip += PlayerHandler.TogglingNoClip;
            Server.WaitingForPlayers += ServerHandler.WaitingForPlayers;
        }

        public void UnregisterEvents()
        {
            Player.ReloadingWeapon -= PlayerHandler.ReloadingWeapon;
            Player.Shooting -= PlayerHandler.Shooting;
            Player.UnloadingWeapon -= PlayerHandler.UnloadingWeapon;
            Player.TogglingNoClip -= PlayerHandler.TogglingNoClip;
            Server.WaitingForPlayers -= ServerHandler.WaitingForPlayers;


            ServerHandler = null;
            PlayerHandler = null;
        }
    }
}

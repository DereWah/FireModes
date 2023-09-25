using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.Generic;
using Exiled.Events;
using FireModes.Types;
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

        public static Main Singleton;

        public Dictionary<ushort, WeaponData> WeaponMemory = new Dictionary<ushort, WeaponData>();


        public override void OnEnabled()
        {
            base.OnEnabled();
            Singleton = this;
            RegisterEvents();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            base.OnDisabled();
        }

        public void RegisterEvents()
        {
            ServerHandler = new EventHandlers.ServerHandler();
            PlayerHandler = new EventHandlers.PlayerHandler();

            Player.ReloadingWeapon += PlayerHandler.ReloadingWeapon;
            Player.Shooting += PlayerHandler.Shooting;
            Player.UnloadingWeapon += PlayerHandler.UnloadingWeapon;
            Player.TogglingNoClip += PlayerHandler.TogglingNoClip;
            Server.RoundStarted += ServerHandler.RoundStarted;
        }

        public void UnregisterEvents()
        {
            Player.ReloadingWeapon -= PlayerHandler.ReloadingWeapon;
            Player.Shooting -= PlayerHandler.Shooting;
            Player.UnloadingWeapon -= PlayerHandler.UnloadingWeapon;
            Player.TogglingNoClip -= PlayerHandler.TogglingNoClip;
            Server.RoundStarted -= ServerHandler.RoundStarted;


            ServerHandler = null;
            PlayerHandler = null;
        }
    }
}

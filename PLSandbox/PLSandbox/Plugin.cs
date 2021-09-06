using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLSandbox
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance;
        public override string Author { get; } = "KuebV";
        public override string Name { get; } = "Peanut's Laboratory Sandbox";
        public override string Prefix { get; } = "PLSandbox";
        public EventHandlers EventHandlers { get; private set; }

        public bool DamageShapeShift;
        public bool JumpDamage;

        public Dictionary<Player, float> TimesJumped = new Dictionary<Player, float>();


        public override void OnEnabled()
        {
            Instance = this;
            EventHandlers = new EventHandlers(this);

            Exiled.Events.Handlers.Player.ChangingRole += EventHandlers.OnChangeClass;
            Exiled.Events.Handlers.Player.Hurting += EventHandlers.OnDamage;
            Exiled.Events.Handlers.Server.EndingRound += EventHandlers.RoundEnd;
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.RoundStart;
            Exiled.Events.Handlers.Player.Joined += EventHandlers.OnFinalJoin;


            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;

            Exiled.Events.Handlers.Player.ChangingRole -= EventHandlers.OnChangeClass;
            Exiled.Events.Handlers.Player.Hurting -= EventHandlers.OnDamage;
            Exiled.Events.Handlers.Server.EndingRound -= EventHandlers.RoundEnd;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.RoundStart;
            Exiled.Events.Handlers.Player.Joined -= EventHandlers.OnFinalJoin;

            base.OnDisabled();
        }

    }
}

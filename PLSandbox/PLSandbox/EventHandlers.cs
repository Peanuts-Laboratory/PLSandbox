using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.Events.EventArgs;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MEC;
using Exiled.API.Extensions;

namespace PLSandbox
{
    public class EventHandlers
    {
        private readonly Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        private static List<string> RainbowHexColors = new List<string> {
            "#DB7476", "#ED8C37", "#F5A936", "#FDFD97", "#9EE09E", "#51A885", "#9EC1CF", "#267A9E", "#986B9B", "#9EC1CF", "#CC99C9"};

        public static void RainbowBroadcast(string Message, ushort duration)
        {
            string stringBuilder = "";
            int index = 0;
            foreach (char stringChar in Message)
            {
                string color = RainbowHexColors.ElementAt(index);
                stringBuilder += $"<color={color}>{stringChar}</color>";

                if (!char.IsWhiteSpace(stringChar))
                    index++;

                if (index >= 10)
                    index = 0;
            }

            Map.Broadcast(duration, stringBuilder, Broadcast.BroadcastFlags.Normal, false);

        }

        public static void SendSettingChange(string message, string color)
        {
            foreach (Player player in Player.List)
            {
                player.SendConsoleMessage(message, color);
            }
        }

        public void OnDamage(HurtingEventArgs ev)
        {
            if (plugin.DamageShapeShift)
            {
                Player player = ev.Target;
                System.Random rand = new System.Random();
                double range = 1.2 - 0.6;

                List<float> values = new List<float>();

                for (int i = 0; i < 3; i++)
                {
                    double samp = rand.NextDouble();
                    double scaled = (samp * range) + 0.6;
                    float f = (float)scaled;
                    values.Add(f);
                }

                player.Scale = new Vector3(values.ElementAt(0), values.ElementAt(1), values.ElementAt(2));
                values.Clear();
            }

            if (!Round.IsStarted)
                return;

            
            
        }

        public void OnPlayerShot(ShotEventArgs ev)
        {
            if (plugin.HaloSwat)
            {
                Player Target = Player.Get(ev.Target);
                if (IsHeadshot(ev.HitboxTypeEnum) && Target.Team != Team.SCP)
                {
                    ev.Damage = Target.Health;
                    return;
                }

                else if (Target.Team != Team.SCP)
                {
                    float damageHalf = ev.Damage / 2.5f;
                    ev.Damage = damageHalf;
                    return;
                }
            }
        }

        private bool IsHeadshot(HitBoxType HitPosition) => HitPosition == HitBoxType.HEAD;

        public void OnChangeClass(ChangingRoleEventArgs ev)
        {
            Log.Debug(null);
        }

        public void OnFinalJoin(JoinedEventArgs ev) => plugin.TimesJumped.Add(ev.Player, 0.01f);

        public void RoundStart()
        {
            Timing.RunCoroutine(UpdateEverySecond(), "UpdateEverySecond");
            foreach (Player ply in Player.List)
            {
                if (ply.Nickname.Contains(plugin.Config.AnomalyName))
                {
                    ply.Broadcast(10, "<b><color=red>You have become an anomaly\nPress (`) for more info</color></b>", Broadcast.BroadcastFlags.Monospaced, true);
                    ply.SendConsoleMessage("\nYou have found an anomaly secret!\nDM KuebV#0111 on the Discord for a prize!\nDo not share this with others", "red");
                }
            }
        }
        public void RoundEnd(EndingRoundEventArgs ev)
        {
            Timing.KillCoroutines("UpdateEverySecond");
            Timing.KillCoroutines("EmergencyBroadcast");
            plugin.TimesJumped.Clear();

            plugin.JumpDamage = false;
            plugin.DamageShapeShift = false;

        }

        public static int SizeBroadcastCorrection(string message)
        {
            if (message.Length > 38)
                return 40;
            if (message.Length > 58)
                return 30;

            return 50;
        }

        public static IEnumerator<float> EmergencyBroadcast(string Message)
        {
            int broadcastSize = SizeBroadcastCorrection(Message);
            while (Plugin.Instance.EmergencyBroadcastActive)
            {
                foreach (Player ply in Player.List)
                {
                    ply.Broadcast(3, $"<b><color=#101010>==[</color><color=#EA2D00>EMERGENCY BROADCAST</color><color=#101010>]==</color></b>\n<size={broadcastSize}><color=#E2A81D>{Message}</color></size>", Broadcast.BroadcastFlags.Monospaced, false);
                    ply.Broadcast(3, $"<b><color=#101010>==[</color><color=#EA2D00>EMERGENCY BROADCAST</color><color=#101010>]==</color></b>\n\n<size={broadcastSize}><color=#E2A81D>{Message}</color></size>", Broadcast.BroadcastFlags.Monospaced, false);
                }
                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> UpdateEverySecond()
        {
            // This may cause some stress to the server
            if (plugin.JumpDamage)
            {
                for (; ; )
                {
                    foreach (Player player in Player.List)
                    {
                        if (player.IsJumping)
                        {
                            float currentValue = plugin.TimesJumped[player];
                            int damageToTake = (int)Math.Round(100 * currentValue, 0);

                            player.Health = player.Health - damageToTake;
                            if (player.Health <= 0 || player.Health > player.MaxHealth)
                                player.Kill();

                            plugin.TimesJumped[player] = currentValue * 1.2f;
                        }

                    }
                    yield return Timing.WaitForSeconds(0.5f);
                }
            }

        }
    }
}

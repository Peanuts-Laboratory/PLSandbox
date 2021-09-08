using CommandSystem;
using Exiled.Permissions.Extensions;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLSandbox.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SandboxSettings : ICommand
    {
        public string Command { get; } = "sandboxsettings";

        public string[] Aliases { get; } = new[] { "sset" };

        public string Description { get; } = "Peanut's Laboratory Sandbox Settings";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("sandbox.admin"))
            {
                response = "Permission Denied";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Syntax : sset [setting / list / killall] [true / false]";
                return false;
            }

            switch (arguments.At(0).ToLower())
            {
                case "list":
                    response = "- DamageShapeShift\n" +
                               "- JumpDamage\n" +
                               "- HaloSwat";
                    return false;
                case "killall":
                    Timing.KillCoroutines("UpdateEverySecond");
                    Plugin.Instance.TimesJumped.Clear();

                    Plugin.Instance.JumpDamage = false;
                    Plugin.Instance.DamageShapeShift = false;
                    response = "Killed All Coroutines";
                    return false;

                case "damageshapeshift":
                    Plugin.Instance.DamageShapeShift = bool.Parse(arguments.At(1));

                    EventHandlers.SendSettingChange("PLSandbox : Damage Shape Shift has been set to: " + Plugin.Instance.DamageShapeShift, "magenta");

                    response = "Damage Shape Shift has been set to: " + Plugin.Instance.DamageShapeShift;
                    return false;
                case "jumpdamage":
                    Plugin.Instance.JumpDamage = bool.Parse(arguments.At(1));

                    EventHandlers.SendSettingChange("PLSandbox : Jump Damage has been set to: " + Plugin.Instance.JumpDamage, "magenta");
                    response = "PLSandbox : Jump Damage has been set to: " + Plugin.Instance.JumpDamage;
                    return false;

                case "haloswat":
                    Plugin.Instance.HaloSwat = bool.Parse(arguments.At(1));
                    EventHandlers.SendSettingChange("PLSandbox : Halo Swat has been set to: " + Plugin.Instance.HaloSwat, "magenta");
                    response = "PLSandbox : Halo Swat has been set to: " + Plugin.Instance.HaloSwat;
                    return false;
                default:
                    response = "Unknown Setting";
                    return false;
            }
        }
    }
}

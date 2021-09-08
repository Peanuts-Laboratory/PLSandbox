using CommandSystem;
using Exiled.API.Features;
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
    public class EmergencyBroadcast : ICommand
    {
        public string Command { get; } = "emergencybc";

        public string[] Aliases { get; } = new[] { "ebc" };

        public string Description { get; } = "Emergency Broadcast";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("sandbox.rbc"))
            {
                response = "Permission Denied";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Syntax : ebc [start / stop] [message]";
                return false;
            }

            switch (arguments.At(0).ToLower())
            {
                case "start":
                    Plugin.Instance.EmergencyBroadcastActive = true;

                    string message = String.Join(" ", arguments.Array, 2, arguments.Array.Length - 2);
                    Timing.RunCoroutine(EventHandlers.EmergencyBroadcast(message), "EmergencyBroadcast");
                    response = "Emergency Broadcast has Started!";
                    return true;
                case "stop":
                    Map.ClearBroadcasts();
                    Timing.KillCoroutines("EmergencyBroadcast");
                    Plugin.Instance.EmergencyBroadcastActive = false;

                    response = "Stopping Emergency Broadcast!";
                    return true;
                default:
                    response = "Unknown Action";
                    return true;
            }
        }
    }
}

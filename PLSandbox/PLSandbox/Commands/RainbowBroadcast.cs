using CommandSystem;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLSandbox.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RainbowBroadcast : ICommand
    {
        public string Command { get; } = "rainbowbc";

        public string[] Aliases { get; } = new[] { "rbc" };

        public string Description { get; } = "Rainbow Broadcast";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("sandbox.rbc"))
            {
                response = "Permission Denied";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Syntax : rbc [time] [message]";
                return false;
            }

            string message = String.Join(" ", arguments.Array, 2, arguments.Array.Length - 2);
            ushort duration = (ushort)Convert.ToInt16(arguments.At(0));

            EventHandlers.RainbowBroadcast(message, duration);
            response = "Done!";
            return true;

        }
        
}
}

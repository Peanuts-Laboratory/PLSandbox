using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLSandbox.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SendConsoleMessage : ICommand
    {
        public string Command { get; } = "sendconsolemessage";

        public string[] Aliases { get; } = new[] { "scm" };

        public string Description { get; } = "Send a console message to everyone";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("sandbox.admin"))
            {
                response = "Permission Denied";
                return false;
            }

            string message = String.Join(" ", arguments.Array, 1, arguments.Array.Length - 1);
            foreach(Player player in Player.List)
            {
                player.SendConsoleMessage(message, "magenta");
            }

            response = "<sprite=3> It has been done.";
            return false;
        }
    }
}

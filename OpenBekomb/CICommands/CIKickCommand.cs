using CommandInterpreter;
using System.Linq;

namespace OpenBekomb.CICommands
{
    [Command("kick")]
    class CIKickCommand : ACommand
    {
        public CIKickCommand()
        {
        }

        public override string ManPage =>
@"Sends a message to an irc user
Takes 2 parameters";

        [Runnable]
        public void RunCommand(string _channel, string _user)
        {
            RunCommand(_channel, _user, null);
        }

        [Runnable]
        public void RunCommand(string _channel, string _user, string _reason)
        {
            ABot.Bot.Kick(_channel, _user, _reason);
        }
    }
}

using CommandInterpreter;
using System.Linq;

namespace OpenBekomb.CICommands
{
    [Command("part")]
    public sealed class CIPartCommand : ACommand
    {
        public CIPartCommand()
        {
        }

        public override string ManPage =>
@"Parts an irc channel
Takes ";

        [Runnable]
        public void RunCommand(string _channel)
        {
            RunCommand(_channel, null);
        }

        [Runnable]
        public void RunCommand(string _channel, string _reason)
        {
            ABot.Bot.Part(_channel, _reason);
        }
    }
}

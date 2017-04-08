using System.Linq;
using CommandInterpreter;

namespace OpenBekomb.CICommands
{
    [Command("send")]
    public class CISendCommand : ACommand
    {
        public CISendCommand()
        {
        }

        public override string ManPage
        {
            get
            {
                return "Sends a message to an irc user";
            }
        }

        public override void Run(string[] _arguments)
        {
            base.Run(_arguments);
            if (_arguments.Length < 2)
            {
                m_owner.InvokeError("you need at least 2 parameters");
                return;
            }

            ABot.Bot.SendMessage(_arguments[0], string.Join(",", _arguments.Skip(1).ToArray()));
        }
    }
}

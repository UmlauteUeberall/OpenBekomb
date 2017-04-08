using System.Linq;
using CommandInterpreter;

namespace OpenBekomb.CICommands
{
    [Command("join")]
    public class CIJoinCommand : ACommand
    {
        public CIJoinCommand()
        {
        }

        public override string ManPage
        {
            get
            {
                return "Joins into an irc channel";
            }
        }

        public override void Run(string[] _arguments)
        {
            base.Run(_arguments);
            if (_arguments.Length < 1)
            {
                m_owner.InvokeError("you need at least 1 parameter");
                return;
            }

            ABot.Bot.Join(_arguments[0]);
        }
    }
}

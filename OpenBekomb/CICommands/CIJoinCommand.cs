using CommandInterpreter;

namespace OpenBekomb.CICommands
{
    [Command("join")]
    public class CIJoinCommand : ACommand
    {
        public CIJoinCommand()
        {
        }

        public override string ManPage => 
@"Joins into an irc channel
Needs one parameter";

        public override void Run(string[] _arguments)
        {
            base.Run(_arguments);
            if (_arguments.Length != 1)
            {
                m_owner.InvokeError("you need at 1 parameter");
                return;
            }

            ABot.Bot.Join(_arguments[0]);
        }
    }
}

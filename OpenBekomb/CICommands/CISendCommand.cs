using System.Linq;
using CommandInterpreter;
using plib.Util;

namespace OpenBekomb.CICommands
{
    [Command("send")]
    public class CISendCommand : ACommand
    {
        public CISendCommand()
        {
        }

        public override string ManPage => 
@"Sends a message to an irc user
Takes to parameters";
       
        public override void Run(string[] _arguments)
        {
            base.Run(_arguments);
            if (_arguments.Length < 2)
            {
                m_owner.InvokeError("you need at least 2 parameters");
                return;
            }

            string message = string.Join(",", _arguments.Skip(1).ToArray());

            message = m_owner.CheckVariables(message);

            message.Split(new[] { "\r\n" }, System.StringSplitOptions.None).
                ForEach(o => ABot.Bot.SendMessage(_arguments[0], o));
        }
    }
}

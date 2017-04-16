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
            if (_arguments.Length < 1)
            {
                m_owner.InvokeError("you need at least 1 parameter");
                return;
            }
            string message;
            if (_arguments.Length == 1)
            {
                message = _arguments[0];
            }
            else
            {
                message = string.Join(", ", _arguments.Skip(1).ToArray());
            }

            string target = _arguments.Length == 1 
                        ? m_owner.Variables["$SENDER"].m_Value 
                        : _arguments[0];

            message = m_owner.CheckVariables(message);

            string[] lines = message.Split(new[] { "\r\n" }, System.StringSplitOptions.None);
            lines.ForEach(o => ABot.Bot.SendMessage(target, o));
        }
    }
}

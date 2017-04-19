using CommandInterpreter;
using plib.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.CICommands
{
    [Command("run")]
    class CIRunCommand : ACommand
    {
        public override string ManPage => 
@"Runs a CI-Command to a target
Takes two parameters";

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

            //message = m_owner.CheckVariables(message);
            CmdInterpreter ci = m_owner.Clone();
            ci.Initialize(System.Console.WriteLine);
            message = ci.Run(message);

            string[] lines = message.Split(new[] { "\r\n" }, System.StringSplitOptions.None);
            lines.ForEach(o => ABot.Bot.SendMessage(target, o));
        }
    }
}

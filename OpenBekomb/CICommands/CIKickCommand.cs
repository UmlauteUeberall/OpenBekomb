using CommandInterpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            ABot.Bot.Kick(_arguments[0], _arguments[1], _arguments.Length > 2 
                                    ? string.Join(",",_arguments.Skip(2).ToArray()) 
                                    : null);

            //message.Split(new[] { "\r\n" }, System.StringSplitOptions.None).
            //    ForEach(o => ABot.Bot.SendMessage(_arguments[0], o));
        }
    }
}

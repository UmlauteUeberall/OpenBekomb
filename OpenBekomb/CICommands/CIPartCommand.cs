using CommandInterpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.CICommands
{
    [Command("part")]
    class CIPartCommand : ACommand
    {
        public CIPartCommand()
        {
        }

        public override string ManPage => 
@"Parts an irc channel
Takes ";

        public override void Run(string[] _arguments)
        {
            base.Run(_arguments);
            if (_arguments.Length < 1)
            {
                m_owner.InvokeError("you need at least 1 parameter");
                return;
            }

            ABot.Bot.Part(_arguments[0], _arguments.Length > 1 
                ? string.Join(",",_arguments.Skip(1).ToArray() ) 
                : null);
        }
    }
}

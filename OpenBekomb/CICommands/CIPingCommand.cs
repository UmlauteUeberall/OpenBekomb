using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandInterpreter;

namespace OpenBekomb.CICommands
{
    [Command("ping")]
    class CIPingCommand : ACommand
    {
        public override string ManPage =>
@"Shows latency to the server
takes no parameters";

        public override void Run(string[] _arguments)
        {
            base.Run(_arguments);

            ABot.Bot.SendRawMessage($"PING :{DateTime.Now.Ticks}");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandInterpreter;

namespace OpenBekomb.CICommands
{
    class CIPingCommand : ACommand
    {
        public override string ManPage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Run(string[] _arguments)
        {
            base.Run(_arguments);

            ABot.Bot.SendRawMessage($"PING :{DateTime.Now.Ticks}");
        }
    }
}

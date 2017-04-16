using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandInterpreter;
using ddate;
using OpenBekomb;

namespace SimpleBot.CICommands
{
    [Command("ddate")]
    class CIDDateCommand : ACommand
    {
        public override string ManPage => "";

        public override void Run(string[] _arguments)
        {
            base.Run(_arguments);

            DDay day = DateTime.Now.Eristify();

            ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, 
                                day.ToString(EFormat.FULL));
        }
    }
}

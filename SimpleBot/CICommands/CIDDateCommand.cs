﻿using CommandInterpreter;
using ddate;
using OpenBekomb;

namespace SimpleBot.CICommands
{
    [Command("ddate")]
    class CIDDateCommand : ACommand
    {
        public override string ManPage => "ddates input or current Time";

        public override void Run(string[] _arguments)
        {
            base.Run(_arguments);



            DDay day;
            if (_arguments.Length > 0)
            {
                try
                {
                    day = System.DateTime.Parse(string.Join(",", _arguments)).Eristify();
                    ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value,
                                day.ToString(EFormat.LONG_DATE));
                }
                catch (System.Exception _ex)
                {
                    m_owner.InvokeError("DateTime Format kaputt: " + _ex);
                    return;
                }
            }
            else
            {
                day = System.DateTime.Now.Eristify();
                ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value,
                                    day.ToString(EFormat.FULL));
            }

        }
    }
}

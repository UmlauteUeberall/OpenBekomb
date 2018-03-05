using CommandInterpreter;
using System;
using System.Collections.Generic;
using plib.Util;
using OpenBekomb;

namespace SimpleBot.CICommands
{
    [Command("split-years")]
    public sealed class CSplitYearsCommand : ACommand
    {
        public override string ManPage => "output years in years, month day and so";

        [Runnable]
        public void RunCommand(float _value)
        {
            string text = MathHelper.GetYearsWithSplits(_value, MathHelper.mu_yearsSplitDefaultConversionGerman);
            foreach (string s in text.Split('\n'))
            {
                ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, s);
            }
        }
    }
}

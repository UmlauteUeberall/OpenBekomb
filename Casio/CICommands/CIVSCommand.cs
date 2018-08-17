using CommandInterpreter;
using OpenBekomb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casio.CICommands
{
    [Command("vs")]
    public sealed class CIVSCommand : ACommand
    {
        public override string ManPage => "";

        [Runnable]
        public void RunCommand(string _word1, string _word2)
        {
            CKCThread[] threads = CKohlchanParser.Parse();
            if (threads == null)
            {
                ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, $"kohlchan is down");
                return;
            }
            int count1 = CKohlchanParser.FindCountInThreads(threads, _word1);
            int count2 = CKohlchanParser.FindCountInThreads(threads, _word2);

            ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, $"{_word1} vs {_word2} : {count1} vs {count2}");
        }
    }
}

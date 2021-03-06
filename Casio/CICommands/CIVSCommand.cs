﻿using CommandInterpreter;
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
            CKCThread[] threads = CKohlchanParser.Parse("b");
            if (threads == null)
            {
                ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, $"kohlchan is down");
                m_owner.InvokeError($"kohlchan is down");
                return;
            }
            int count1 = CKohlchanParser.FindCountInThreads(threads, _word1);
            int count2 = CKohlchanParser.FindCountInThreads(threads, _word2);

            ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, $"{_word1} vs {_word2} : {count1} vs {count2}");
        }

        [Runnable]
        public void RunCommand(string _board, string _word1, string _word2)
        {
            if (!CKCThread.mu_validBoards.Contains(_board))
            {
                ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, $"board {_board} does not exist");
                m_owner.InvokeError($"board {_board} does not exist");
                return;
            }

            CKCThread[] threads = CKohlchanParser.Parse(_board);
            if (threads == null)
            {
                ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, $"kohlchan is down");
                m_owner.InvokeError($"kohlchan is down");
                return;
            }
            int count1 = CKohlchanParser.FindCountInThreads(threads, _word1);
            int count2 = CKohlchanParser.FindCountInThreads(threads, _word2);

            ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, $"{_word1} vs {_word2} on {_board} : {count1} vs {count2}");
        }
    }
}

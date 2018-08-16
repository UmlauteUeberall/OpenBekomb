using CommandInterpreter;
using OpenBekomb;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casio.CICommands
{
    [Command("search")]
    public sealed class CSearchCommand : ACommand
    {
        public override string ManPage => "";

        [Runnable]
        public void RunCommand(string _word)
        {
            CKCThread[] threads = CKohlchanParser.Parse();
            CKCThread[] found = CKohlchanParser.FindWordInThreads(threads, _word);
            string text = string.Join(" ", found.Select(o => $"https://kohlchan.net/{o.mu_board}/res/{o.mu_id}.html").ToArray()); 

            ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, $"{_word} found in: {text}");
        }
    }
}

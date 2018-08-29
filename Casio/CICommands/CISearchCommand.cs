using CommandInterpreter;
using OpenBekomb;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casio.CICommands
{
    [Command("search")]
    public sealed class CISearchCommand : ACommand
    {
        public override string ManPage => "";

        [Runnable("searches for this pattern on a board, first part is the board", EEntryPointType.FALLBACK)]
        public void RunCommand(string[] _args = null)
        {
            string board = "b";
            string search = string.Join(", ",_args);

            if (_args.Length > 1 && CKCThread.mu_validBoards.Contains(_args[0]))
            {
                board = _args[0];
                _args = _args.Skip(1).ToArray();
                search = string.Join(", ", _args);
            }

            CKCThread[] threads = CKohlchanParser.Parse(board);
            if (threads == null)
            {
                ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, $"kohlchan is down");
                m_owner.InvokeError($"kohlchan is down");
                return;
            }
            CKCThread[] found = CKohlchanParser.FindWordInThreads(threads, search);
            string text = string.Join(" ", found.Select(o => $"https://kohlchan.net/{o.mu_board}/res/{o.mu_id}.html").ToArray()); 

            ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, $"{search} found in: {text}");
        }
    }
}

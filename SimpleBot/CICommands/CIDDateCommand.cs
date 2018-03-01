using CommandInterpreter;
using ddate;
using OpenBekomb;
using plib.Util;

namespace SimpleBot.CICommands
{
    [Command("ddate")]
    public sealed class CIDDateCommand : ACommand
    {
        public override string ManPage => "ddates input or current Time";

        [Runnable]
        public void RunCommand()
        {
            fi_ddateOutput(System.DateTime.Now.Eristify(), EFormat.FULL, true);
        }

        [Runnable]
        public void RunCommand(System.DateTime _time)
        {
            fi_ddateOutput(_time.Eristify(), EFormat.LONG_DATE);

        }

        private void fi_ddateOutput(DDay _day, EFormat _format, bool _readTTS = false)
        {
            string[] lines = _day.ToString(_format).Split('\n');
            lines.ForEach(o => ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, o));
            if (_readTTS)
            {
                lines.ForEach(o => ABot.Bot.Mod<Modules.TTSModule>().AddMessage(o, Modules.TTSModule.ETTSLanguage.ENGLISH));
            }
        }
    }
}

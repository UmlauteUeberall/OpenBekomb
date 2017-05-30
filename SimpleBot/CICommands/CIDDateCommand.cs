using CommandInterpreter;
using ddate;
using OpenBekomb;
using plib.Util;

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
            EFormat format;
            if (_arguments.Length > 0)
            {
                try
                {
                    day = System.DateTime.Parse(string.Join(",", _arguments)).Eristify();
                    format = EFormat.LONG_DATE;
                    
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
                format = EFormat.FULL;
            }

            string[] lines = day.ToString(format).Split('\n');
            lines.ForEach(o => ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, o));

        }
    }
}

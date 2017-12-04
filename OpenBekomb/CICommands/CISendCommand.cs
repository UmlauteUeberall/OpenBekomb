using System.Linq;
using CommandInterpreter;
using plib.Util;

namespace OpenBekomb.CICommands
{
    [Command("send")]
    public sealed class CISendCommand : ACommand
    {
        public CISendCommand()
        {
        }

        public override string ManPage => 
@"Sends a message to an irc user
Takes one to two parameters";

        [Runnable]
        public void RunCommand(string _text)
        {
            string target = m_owner.Variables["$SENDER"].m_Value;
            RunCommand(target, _text);
        }

        [Runnable]
        public void RunCommand(string _target, string _text)
        {
            _text = m_owner.CheckVariables(_text);

            string[] lines = _text.Split(new[] { "\r\n" }, System.StringSplitOptions.None);
            lines.ForEach(o => ABot.Bot.SendMessage(_target, o));

        }
    }
}

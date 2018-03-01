using CommandInterpreter;
using plib.Util;
using System.Linq;

namespace OpenBekomb.CICommands
{
    [Command("run")]
    class CIRunCommand : ACommand
    {
        public override string ManPage =>
@"Runs a CI-Command to a target
Takes one ore two parameters";
        [Runnable]
        public void RunCommand(string _command)
        {
            string target = m_owner.Variables["$SENDER"].m_Value;

            fi_runCICommand(target, _command);
        }

        [Runnable]
        public void RunCommand(string _target, string _command)
        {
            m_owner.AddVariable("$SENDER", _target, true);

            fi_runCICommand(_target, _command);

        }

        private void fi_runCICommand(string _target, string _command)
        {
            CmdInterpreter ci = m_owner.Clone();
            ci.Initialize(System.Console.WriteLine);
            _command = ci.Run(_command);

            string[] lines = _command.Split(new[] { "\r\n" }, System.StringSplitOptions.None);
            lines.ForEach(o => ABot.Bot.SendMessage(_target, o));
        }
    }
}

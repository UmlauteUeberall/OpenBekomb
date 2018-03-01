using CommandInterpreter;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using plib.Util;
using OpenBekomb;
using SimpleBot.Modules;
using SimpleBot.Modules.Cron;

namespace SimpleBot.CICommands
{
    [Command("cron")]
    public sealed class CICronCommand : ACommand
    {
        public override string ManPage =>
@"adds a cronjob
uses discordian date";

        [Runnable]
        public void RunCommand()
        {
            string info = ABot.Bot.Mod<CronModule>().GetCronInfo();
            info.Split('\n').
                ForEach(o => ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, o));
        }

        [Runnable]
        public void RunCommand(int _idToDelete)
        {
            ABot.Bot.Mod<CronModule>().RemoveCron(_idToDelete);
        }

        [Runnable]
        public void RunCommand(string _time1, string _command)
        {
            fi_AddCron(_command, _time1);
        }

        [Runnable]
        public void RunCommand(string _time1, string _time2, string _command)
        {
            fi_AddCron(_command, _time1, _time2);
        }

        [Runnable]
        public void RunCommand(string _time1, string _time2, string _time3, string _command)
        {
            fi_AddCron(_command, _time1, _time2, _time3);
        }

        [Runnable]
        public void RunCommand(string _time1, string _time2, string _time3, string _time4, string _command)
        {
            fi_AddCron(_command, _time1, _time2, _time3, _time4);
        }

        [Runnable]
        public void RunCommand(string _time1, string _time2, string _time3, string _time4, string _time5, string _command)
        {
            fi_AddCron(_command, _time1, _time2, _time3, _time4, _time5);
        }

        private void fi_AddCron(string _command, params string[] _times)
        {
            CronModule cm = ABot.Bot.Mod<CronModule>();

            Queue<CronOption> options = new Queue<CronOption>();
            Regex r = new Regex("^([0-9*]+)([mhdMD])$");
            Match m;
            _times.ForEach(o =>
            {
                m = r.Match(o);
                if (!m.Success)
                {
                    m_owner.InvokeError($"timestamp {o} is not valid, it must be formatted like \"^([0-9]+)([mhdMD])$\"");
                    throw new ReturnException();
                }

                if (m.Groups[1].Value != "*")
                {
                    options.Enqueue(new CronOption(m.Groups[2].Value, int.Parse(m.Groups[1].Value)));

                }
            });

            cm.AddCron(new CronEntry(options, _command));
        }
    }
}

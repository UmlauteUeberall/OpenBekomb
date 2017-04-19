using CommandInterpreter;
using OpenBekomb;
using plib.Util;
using SimpleBot.Modules;
using SimpleBot.Modules.Cron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleBot.CICommands
{
    [Command("cron")]
    class CICronCommand : ACommand
    {
        public override string ManPage => 
@"adds a cronjob
uses discordian date";

        public override void Run(string[] _arguments)
        {
            base.Run(_arguments);

            CronModule cm = ABot.Bot.Mod<CronModule>();

            if (_arguments.Length == 0)
            {
                string info = cm.GetCronInfo();
                info.Split('\n').
                    ForEach(o => ABot.Bot.SendMessage(m_owner.Variables["$SENDER"].m_Value, o));
                return;
            }

            Regex r = new Regex("^[0-9]+$");
            Match m;
            if (_arguments.Length == 1 && (m = r.Match(_arguments[0])).Success)
            {
                cm.RemoveCron(int.Parse(m.Value));
                return;
            }

            Queue<CronOption> options = new Queue<CronOption>();
            r = new Regex("^([0-9]+)([mhdMD])$");
            int i = 0;
            for (; i < _arguments.Length; i++)
            {
                m = r.Match(_arguments[i]);
                if (!m.Success)
                {
                    break;
                }
                options.Enqueue(new CronOption(m.Groups[2].Value, int.Parse(m.Groups[1].Value)));
            }
            string command = string.Join(", ", _arguments.Skip(i).ToArray());

            cm.AddCron(new CronEntry(options, command));
        }

        /*
        public override void Answer(Channel _chan, User _sender, User _target, string _message)
        {
            base.Answer(_chan, _sender, _target, _message);

            if (Owner.IsAdressed(_message))
            {
                string[] words = _message.Split(' ');
                if (words.Length > 2 && words[1] == Name)
                {
                    CreateCron(string.Join(" ", words.Skip(2).ToArray()));
                }
            }
        }

        public void CreateCron(string _crontext)
        {
            Queue<string> words = new Queue<string>(_crontext.Split(' '));
            List<CronOption> options = new List<CronOption>(5);
            Match match;

            string currentWord;
            while (words.Count > 0)
            {
                currentWord = words.Dequeue();
                match = Regex.Match(currentWord, @"^(\d+)([mhdMD])$");
                if (!match.Success)
                {
                    break;
                }

                if (options.Any(o => o.Name == match.Groups[2].Value))
                {
                    break;
                }
                options.Add(new CronOption(match.Groups[2].Value, int.Parse(match.Groups[1].Value)));
            }

            if (words.Count > 0)
            {
                m_cronJobs.Add(new CronEntry(options, string.Join(" ", words.ToArray())));
            }

        }
        */
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBekomb;
using OpenBekomb.Modules;
using System.Text.RegularExpressions;
using SimpleBot.Modules.Cron;

namespace SimpleBot.Modules
{
    public class CronModule : AModule
    {
        private List<CronEntry> m_cronJobs = new List<CronEntry>();

        public CronModule(ABot _bot) 
            : base(_bot)
        {
        }

        public override string Name
        {
            get
            {
                return "cron";
            }
        }

        public override void Update(float _deltaTime)
        {
            base.Update(_deltaTime);
        }


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
    }
}

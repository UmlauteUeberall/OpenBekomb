using System;
using System.Collections.Generic;
using OpenBekomb;
using OpenBekomb.Modules;
using SimpleBot.Modules.Cron;

namespace SimpleBot.Modules
{
    public class CronModule : AModule
    {
        private List<CronEntry> m_cronJobs = new List<CronEntry>();

        private int m_lastMinRun = -1;

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

            DateTime d = DateTime.Now;

            if (d.Minute != m_lastMinRun)
            {
               m_lastMinRun = d.Minute;

                foreach (CronEntry ce in m_cronJobs)
                {
                    if (ce.IsReady(d))
                    {
                        ce.Fire();
                    }
                }
            }
        }

        internal void AddCron(CronEntry _cronEntry)
        {
            m_cronJobs.Add(_cronEntry);
        }

        internal void RemoveCron(int _pos)
        {
            if (m_cronJobs.Count > _pos)
            {
                m_cronJobs.RemoveAt(_pos);
            }
        }

        internal string GetCronInfo()
        {
            string s = "#    m    h    d    M    D    Command";
            if (m_cronJobs.Count > 0)
            {
                s += "\n";
                for (int i = 0; i < m_cronJobs.Count; i++)
                {
                    s += i + "    " + m_cronJobs[i].ToString() + "\n";
                }
                //m_cronJobs.Select(o => o.ToString()).Aggregate((o1, o2) => o1 + "\n" + o2);
            }

            return s;
        }
    }
}

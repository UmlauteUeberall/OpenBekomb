using ddate;
using OpenBekomb;
using plib.Util;
using System.Collections.Generic;

namespace SimpleBot.Modules.Cron
{
    class CronEntry
    {
        int m_minute = -1;
        int m_hour = -1;
        int m_dom = -1;
        int m_month = -1;
        int m_dow = -1;

        string m_command;


        public CronEntry(IEnumerable<CronOption> _options, string _command)
        {
            Queue<CronOption> os = new Queue<CronOption>(_options);
            CronOption tmp;
            m_command = _command;

            while (os.Count != 0)
            {
                tmp = os.Dequeue();
                switch (tmp.Name)
                {
                    case "m":
                        m_minute = tmp.Value;
                        break;
                    case "h":
                        m_hour = tmp.Value;
                        break;
                    case "d":
                        m_dom = tmp.Value;
                        break;
                    case "M":
                        m_month = tmp.Value;
                        break;
                    case "D":
                        m_dow = tmp.Value;
                        break;
                }
            }
        }

        public bool IsReady(System.DateTime _dateTime)
        {
            DDay d = _dateTime.Eristify();

            return (m_minute == -1 || m_minute == _dateTime.Minute)
                && (m_hour == -1 || m_hour == _dateTime.Hour)
                && (m_dom == -1 || m_dom == d.Day)
                && (m_month == -1 || m_month == d.Month + 1)
                && (m_dow == -1 || m_dow == (int)d.DayOfTheWeek + 1);
        }

        public void Fire()
        {
            ABot.Bot.AddCICommand(m_command.Unescape("\\"));
        }

        public override string ToString()
        {
            return (m_minute == -1 ? "*" : m_minute.ToString()) +"    "
                + (m_hour == -1 ? "*" : m_hour.ToString()) + "    "
                + (m_dom == -1 ? "*" : m_dom.ToString()) + "    "
                + (m_month == -1 ? "*" : m_month.ToString()) + "    "
                + (m_dow == -1 ? "*" : m_dow.ToString()) + "    "
                + m_command;
        }
    }
}

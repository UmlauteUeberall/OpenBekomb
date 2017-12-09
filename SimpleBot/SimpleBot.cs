using OpenBekomb;
using SimpleBot.Modules;
using ddate;

namespace SimpleBot
{
    public class SimpleBot : ABot
    {
        public SimpleBot(string _host, int _port)
            :base (_host, _port, fi_timeFormat)
        {
            AddModule(new TTSModule(this));
            AddModule(new CronModule(this));
        }

        static string fi_timeFormat()
        {
            System.DateTime now = System.DateTime.Now;
            DDay dday = now.Eristify();

            return $"{dday.ToString(EFormat.SHORT_DATE)} {now.Hour:00}:{now.Minute:00}:{now.Second}:00";
        }
    }
}

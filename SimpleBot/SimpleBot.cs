using OpenBekomb;
using SimpleBot.Modules;

namespace SimpleBot
{
    public class SimpleBot : ABot
    {
        public SimpleBot(string _host, int _port)
            :base (_host, _port)
        {
            AddModule(new TTSModule(this));
            AddModule(new CronModule(this));
        }
    }
}

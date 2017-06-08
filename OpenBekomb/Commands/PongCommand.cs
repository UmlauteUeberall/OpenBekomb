using plib.Util;
using OpenBekomb.Modules;

using DateTime = System.DateTime;

namespace OpenBekomb.Commands
{
    class PongCommand : ABotCommand
    {
        public PongCommand(ABot _bot) 
            : base(_bot)
        {
        }

        public override string Name
        {
            get
            {
                return "PONG";
            }
        }

        public override void Answer(string _sender, string _target, string _messageBody)
        {
            L.Log(_messageBody);
            DateTime d = new DateTime(long.Parse(_messageBody));
            //Owner.Mod<PingModule>().LastPingTime = (float) (DateTime.Now - d).TotalMilliseconds;
        }
    }
}

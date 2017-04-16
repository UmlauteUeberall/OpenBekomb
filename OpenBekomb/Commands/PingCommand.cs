using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.Commands
{
    public class PingCommand : ABotCommand
    {
        //private float m_timer;

        public override string Name
        {
            get
            {
                return "PING";
            }
        }

        public PingCommand(ABot _bot) 
            : base(_bot)
        {
        }

        public override void Answer(string _sender, string _target, string _messageBody)
        {
            Owner.SendRawMessage($"PONG :{_messageBody}");
        }
    }
}

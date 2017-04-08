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

        //internal override void Update(float _deltaTime)
        //{
        //    m_timer += _deltaTime;
        //    if (m_timer > 10)
        //    {
        //        //Console.WriteLine(m_timer);
        //        //Owner.SendRawMessage($"PING :{Owner.Name}");
        //        m_timer = 0;
        //    }
        //}

        public override void Answer(string _messageHead, string _messageBody)
        {
            Owner.SendRawMessage($"PONG :{_messageBody}");
        }
    }
}

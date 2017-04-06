using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb
{
    public class PingModule : AModule
    {
        private float m_timer;

        public override string Message
        {
            get
            {
                return "PING";
            }
        }

        public PingModule(ABot _bot) 
            : base(_bot)
        {
        }

        internal override void Update(float _deltaTime)
        {
            m_timer += _deltaTime;
            if (m_timer > 10)
            {
                //Console.WriteLine(m_timer);
                //Owner.SendRawMessage($"PING :{Owner.Name}");
                m_timer = 0;
            }
        }

        public override void Answer(string _message)
        {
            Owner.SendRawMessage($"PONG :{_message.Split(':')[1]}");
        }
    }
}

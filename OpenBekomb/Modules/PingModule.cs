using OpenBekomb.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.Modules
{
    public class PingModule : AModule
    {
        private float m_timer;

        public float LastPingTime;

        public PingModule(ABot _bot) 
            : base(_bot)
        {
        }

        public override string Name
        {
            get
            {
                return "ping";
            }
        }

        public override void Update(float _deltaTime)
        {
            m_timer -= _deltaTime;
            if (m_timer <= 0)
            {
                Owner.SendRawMessage($"PING :{DateTime.Now.Ticks}");
                m_timer = 30;
            }
        }

        //public override void Answer(Channel _chan, User _sender, User _target, string _message)
        //{
        //    
        //}


    }
}

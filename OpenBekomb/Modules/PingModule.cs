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

        public override void Answer(Channel _chan, User _sender, User _target, string _message)
        {
            base.Answer(_chan, _sender, _target, _message);

            if (Owner.IsAdressed(_message))
            {
                string[] words = _message.Split(' ');
                if (words.Length > 1 && words[1] == Name)
                {
                    Owner.SendRawMessage($"PING :{DateTime.Now.Ticks}");
                    Owner.SendMessage(_chan?.Name ?? _sender.Name, 
                        $"{_sender.Name}: {LastPingTime}ms");
                }
            }
        }


    }
}

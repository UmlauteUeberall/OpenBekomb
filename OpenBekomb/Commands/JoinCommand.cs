using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.Commands
{
    class JoinCommand : ABotCommand
    {
        public JoinCommand(ABot _bot) : base(_bot)
        {

        }

        public override string Name => "JOIN";

        public override void Answer(string _sender, string _target, string _messageBody)
        {
            string sender = _sender.Split('!')[0];

            if (sender == Owner.m_Name)
            {
                Channel c = Owner.GetChannel(_messageBody) ?? new Channel(_messageBody);
                Owner.HasJoined(c);
            }
            else
            {
                Channel c = Owner.GetChannel(_messageBody);
                c.mu_users.Add(Owner.GetUser(sender));
            }
        }
    }
}

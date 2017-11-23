using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.Commands
{
    class PartCommand : ABotCommand
    {
        public PartCommand(ABot _bot) : base(_bot)
        {
        }

        public override string Name => "PART";

        public override void Answer(string _sender, string _target, string _messageBody)
        {
            string sender = _sender.Split('!')[0];
            Channel c = Owner.GetChannel(_target);
            User u = Owner.GetUser(sender);
            c.mu_users.Remove(u);
        }
    }
}

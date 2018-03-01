using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.Commands
{
    class NicklistRecievedCommand : ABotCommand
    {
        public NicklistRecievedCommand(ABot _bot) : base(_bot)
        {
        }

        public override string Name => "353";

        public override void Answer(string _sender, string _target, string _messageBody)
        {
            Channel c = Owner.GetChannel(_target.Split(' ').Last());
            c.mu_users.Clear();
            c.mu_users.AddRange(_messageBody.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(o => Owner.GetUser(o)));
        }
    }
}

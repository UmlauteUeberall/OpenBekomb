using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.Commands
{
    public class KickCommand : ABotCommand
    {
        public KickCommand(ABot _bot) 
            : base(_bot)
        {
        }

        public override string Name
        {
            get
            {
                return "KICK";
            }
        }

        public override void Answer(string _messageHead, string _messageBody)
        {
            string[] headParts = _messageHead.Split(new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string kickedPerson = headParts.Last();
            if (kickedPerson == Owner.m_Config.m_Name)
            {
                Channel c = Owner.GetChannel(headParts[headParts.Length - 2]);
                Owner.Parted(c);
            }
        }
    }
}

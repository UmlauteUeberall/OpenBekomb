﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.Modules
{
    public class CIModule : AModule
    {
        public CIModule(ABot _bot) : base(_bot)
        {
        }

        public override string Name
        {
            get
            {
                return "ci";
            }
        }

        public override void Answer(Channel _chan, User _sender, User _target, string _message)
        {

            base.Answer(_chan, _sender, _target, _message);

            string[] words = _message.Split(' ');
            if (words.Length < 3 || words[1] != Name)
            {
                return;
            }

            Owner.AddCICommand(string.Join(" ",words.Skip(2).ToArray()));
        }
    }
}

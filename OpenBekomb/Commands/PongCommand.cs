﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using plib.Util;
using OpenBekomb.Modules;

namespace OpenBekomb.Commands
{
    class PongCommand : ABotCommand
    {
        public PongCommand(ABot _bot) 
            : base(_bot)
        {
        }

        public override string Name
        {
            get
            {
                return "PONG";
            }
        }

        public override void Answer(string _messageHead, string _messageBody)
        {
            L.Log(_messageBody);
            DateTime d = new DateTime(long.Parse(_messageBody));

            Owner.Mod<PingModule>().LastPingTime = (float) (DateTime.Now - d).TotalMilliseconds;
        }
    }
}

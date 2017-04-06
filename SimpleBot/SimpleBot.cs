﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBekomb;

namespace SimpleBot
{
    public class SimpleBot : ABot
    {
        public override string Name
        {
            get
            {
                return "SimpleBot";
            }
        }

        public SimpleBot(string _host, int _port)
            :base (_host, _port)
        {

        }
    }
}
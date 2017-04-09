﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleBot.Modules.Cron
{
    class CronOption
    {
        public string Name { get; private set; }
        public int Value { get; private set; }

        public CronOption(string _name, int _value)
        {
            Name = _name;
            Value = _value;
        }
    }
}

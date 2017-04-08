using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb
{
    public class Channel
    {
        public string Name { get; private set; }

        public Channel(string _name)
        {
            Name = _name;
        }
    }
}

using OpenBekomb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casio
{
    class Casio : ABot
    {
        public Casio(string _host, int _port, Func<string> _timeFormatting = null) 
            : base(_host, _port, _timeFormatting)
        {
        }
    }
}

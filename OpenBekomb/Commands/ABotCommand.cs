using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.Commands
{
    public abstract class ABotCommand
    {
        public ABot Owner { get; private set; }

        public abstract string Name { get; }

        public abstract void Answer(string _messageHead, string _messageBody);

        public ABotCommand(ABot _bot)
        {
            Owner = _bot;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb
{
    public abstract class AModule
    {
        public ABot Owner { get; private set; }

        public abstract string Message { get; }

        internal virtual void Update(float _deltaTime)
        {

        }

        public AModule(ABot _bot)
        {
            Owner = _bot;
        }

        public abstract void Answer(string _message);
    }
}

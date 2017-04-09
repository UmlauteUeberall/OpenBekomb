using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.Modules
{
    public abstract class AModule
    {
        public ABot Owner { get; private set; }

        public abstract string Name { get; }

        public virtual void Update(float _deltaTime)
        {

        }

        public virtual void Answer(Channel _chan, User _sender, User _target, string _message)
        {

        }

        public AModule(ABot _bot)
        {
            Owner = _bot;
        }

        
    }
}

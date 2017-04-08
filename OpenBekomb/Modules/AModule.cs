using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb.Modules
{
    public abstract class AModule
    {
        public ABot Owner { get; private set; }

        

        internal virtual void Update(float _deltaTime)
        {

        }

        public AModule(ABot _bot)
        {
            Owner = _bot;
        }

        
    }
}

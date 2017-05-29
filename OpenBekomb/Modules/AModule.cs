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
            //L.Log("Start Process " + Name);
        }

        public AModule(ABot _bot)
        {
            Owner = _bot;
        }

        
    }
}

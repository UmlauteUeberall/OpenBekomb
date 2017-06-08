namespace OpenBekomb.Modules
{
    public abstract class AModule
    {
        public ABot Owner { get; private set; }

        public abstract string Name { get; }

        /// <summary>
        /// May be completely overrriden
        /// </summary>
        /// <param name="_deltaTime"></param>
        public virtual void Update(float _deltaTime)
        {
        }

        /// <summary>
        /// May be completely overrriden, only called in PrivMesgCommand
        /// </summary>
        /// <param name="_deltaTime"></param>
        public virtual void Answer(Channel _chan, User _sender, User _target, string _message)
        {
        }

        public AModule(ABot _bot)
        {
            Owner = _bot;
        }

        
    }
}

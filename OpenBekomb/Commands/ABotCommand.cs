namespace OpenBekomb.Commands
{
    public abstract class ABotCommand
    {
        public ABot Owner { get; private set; }

        public abstract string Name { get; }

        public abstract void Answer(string _sender, string _target, string _messageBody);

        public ABotCommand(ABot _bot)
        {
            Owner = _bot;
        }
    }
}

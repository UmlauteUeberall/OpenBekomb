namespace OpenBekomb
{
    public class Channel
    {
        public string Name { get; private set; }

        public Channel(string _name)
        {
            Name = _name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

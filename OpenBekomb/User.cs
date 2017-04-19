namespace OpenBekomb
{
    public class User
    {
        public string Name { get; private set; }
        public User(string _name)
        {
            Name = _name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

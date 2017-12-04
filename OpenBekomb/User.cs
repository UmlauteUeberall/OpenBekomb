using System.Collections.Generic;
using System.IO;

namespace OpenBekomb
{
    public class User
    {
        public string Name { get; private set; }
        readonly public List<string> mu_log = new List<string>();        // darf nicht von außen befüllt werden

        public User(string _name)
        {
            Name = _name;
        }

        public override string ToString()
        {
            return Name;
        }

        public void AddLog(string _message)
        {
            mu_log.Add(_message);

            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }

            string name = Name.Replace("/", "_");
            File.AppendAllText($"logs/{name}.log", $"[{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]\t{_message}" + System.Environment.NewLine);
        }
    }
}

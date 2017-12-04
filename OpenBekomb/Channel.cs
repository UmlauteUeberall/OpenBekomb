using System.Collections.Generic;
using System.IO;

namespace OpenBekomb
{
    public class Channel
    {
        public string Name { get; private set; }
        readonly public List<User> mu_users = new List<User>();     // Müssen von außen befüllt werden, also in den methoden in denen Nutzer hinzugefügt oder entfernt werden
        readonly public List<LogEntry> mu_log = new List<LogEntry>();

        public Channel(string _name)
        {
            Name = _name;
        }

        public override string ToString()
        {
            return Name;
        }

        internal void LogMessage(User _user, string _messageBody)
        {
            mu_log.Add(new LogEntry(_user, _messageBody));

            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }

            string name = Name.Replace("/","_");
            File.AppendAllText($"logs/{name}.log", $"[{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]\t{_user.Name}\t{_messageBody}" + System.Environment.NewLine);
        }

        public class LogEntry
        {
            public User mu_user;
            public string mu_text;

            public LogEntry(User _user, string _text)
            {
                mu_user = _user;
                mu_text = _text;
            }
        }
    }
}

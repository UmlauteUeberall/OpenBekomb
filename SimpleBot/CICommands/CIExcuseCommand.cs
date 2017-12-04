using CommandInterpreter;
using System.Collections.Generic;
using System.Linq;
using plib.Util;
using System.IO;
using System.Xml.Linq;
using OpenBekomb;


namespace SimpleBot.CICommands
{
    [Command("excuse")]
    sealed class CIExcuseCommand : ACommand
    {
        public override string ManPage =>
@"Returns a excuse
Zero or one tag parameter for output
or 'add' as first parameter followed by tag and text";

        private Dictionary<string, List<string>> mi_excuses;

        public CIExcuseCommand()
        {
            mi_excuses = new Dictionary<string, List<string>>();

            if (File.Exists("excuses.xml"))
            {
                XDocument doc = XDocument.Load("excuses.xml");
                doc.Root.Elements().ForEach(o => mi_excuses.Add((string)o.Attribute("tag"), o.Elements().Select(p => (string)p).ToList()));
            }
            else
            {
                File.Create("excuses.xml");
            }

        }

        [Runnable]
        public void RunCommand(string _command, string _excuser, string _message)
        {
            if (_command == "add")
            {

                fi_addExcuse(_excuser, _message);
            }
            else
            {
                m_owner.InvokeError($"{_command} is not a valid command!");
            }
        }

        [Runnable]
        public void RunCommand()
        {
            RunCommand(mi_excuses.RandomElement().Key);
        }

        [Runnable]
        public void RunCommand(string _excuser)
        {
            string target = m_owner.Variables["$SENDER"].m_Value;
            string message;

            if (mi_excuses.ContainsKey(_excuser))
            {
                message = mi_excuses[_excuser].RandomElement();
            }
            else
            {
                //message = mi_excuses.SelectMany(o => o.Value).RandomElement();
                m_owner.InvokeError($"no excuses found for {_excuser}");
                return;
            }

            message.Split(new[] { @"\n" }, System.StringSplitOptions.RemoveEmptyEntries)
                .ForEach(o => ABot.Bot.SendMessage(target, o));
            
        }

        private void fi_addExcuse(string _tag, string _excuse)
        {
            if (string.IsNullOrEmpty(_tag))
            {
                m_owner.InvokeError("tag cannot be empty");
            }

            if (!mi_excuses.ContainsKey(_tag))
            {
                mi_excuses.Add(_tag, new List<string>());
            }

            mi_excuses[_tag].Add(_excuse);

            XDocument doc = new XDocument();
            doc.Add(new XElement("root"));
            XElement e;
            mi_excuses.ForEach(o =>
            {
                e = new XElement("excuseCat", new XAttribute("tag", o.Key));
                o.Value.ForEach(p => e.Add(new XElement("excuse", new XCData(p))));

                doc.Root.Add(e);
            });

            doc.Save("excuses.xml");
        }
    }
}

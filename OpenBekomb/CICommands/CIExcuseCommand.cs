﻿using CommandInterpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using plib.Util;
using System.IO;
using System.Xml.Linq;

namespace OpenBekomb.CICommands
{
    [Command("excuse")]
    class CIExcuseCommand : ACommand
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
                doc.Root.Elements().ForEach(o => mi_excuses.Add((string)o.Attribute("tag"), o.Elements().Select(p => (string) p).ToList()));
            }
        }

        public override void Run(string[] _arguments)
        {
            base.Run(_arguments);
            //if (_arguments.Length > 2)
            //{
            //    m_owner.InvokeError("you need zero or one parameter parameter for output and 2 parameters for input");
            //    return;
            //}

            if (_arguments.Length > 2 && _arguments[0] == "add")
            {
                fi_addExcuse(_arguments[1], string.Join(",", _arguments.Skip(2).ToArray()));

                return;
            }

            string target = m_owner.Variables["$SENDER"].m_Value;
            string message;
            if (_arguments.Length == 0)
            {
                message = mi_excuses.SelectMany(o => o.Value).RandomElement();
            }
            else
            {
                if (mi_excuses.ContainsKey(_arguments[1]))
                {
                    message = mi_excuses[_arguments[1]].RandomElement();
                }
                else
                {
                    message = mi_excuses.SelectMany(o => o.Value).RandomElement();
                }
            }

            ABot.Bot.SendMessage(target, message);
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
                e = new XElement("excuseCat", new XElement("tag", o.Key));
                o.Value.ForEach(p => e.Add(new XElement("excuse", new XCData(p))));

                doc.Root.Add(e);
            });

            doc.Save("excuses.xml");
        }
    }
}

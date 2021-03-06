﻿using plib.Util;

namespace OpenBekomb.Commands
{
    class PrivMsgCommand : ABotCommand
    {
        public PrivMsgCommand(ABot _bot) 
            : base(_bot)
        {
        }

        public override string Name
        {
            get
            {
                return "PRIVMSG";
            }
        }

        public override void Answer(string _sender, string _target, string _messageBody)
        {
            //string[] headParts = _messageHead.Split(new[] { "PRIVMSG" }, StringSplitOptions.RemoveEmptyEntries);

            //string pattern = @"^:(\w+)";
            //Match m = Regex.Match(headParts[0], pattern);
            string sender = _sender.Split('!')[0];//m.Groups[1].Value.Trim();
            //string target = headParts[1].Trim();

            L.Log(_sender + ":" + _target + ":" + _messageBody);

            if (sender == _target)
            {
                return;
            }


            Channel c = _target.StartsWith("#") ? Owner.GetChannel(_target) : null;
            User s = Owner.GetUser(sender);
            User t = c == null ? new User(_target) : null;

            if (c != null)  // Ist eine Nachricht im Channel
            {
                c.LogMessage(s, _messageBody);
            }
            else // Direktnachricht
            {
                s.AddLog(_messageBody);
            }
            
            Owner.ProcessModulesAnswers(c, s, t, _messageBody);

            //if (target.StartsWith("#"))
            //{
            //    Owner.SendMessage(target, _messageBody.Trim());
            //}
            //else
            //{
            //    Owner.SendMessage(sender, _messageBody.Trim());
            //}
        }
    }
}

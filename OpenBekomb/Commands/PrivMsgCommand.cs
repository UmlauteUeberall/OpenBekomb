using System;
using System.Text.RegularExpressions;

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

        public override void Answer(string _messageHead, string _messageBody)
        {
            string[] headParts = _messageHead.Split(new[] { "PRIVMSG" }, StringSplitOptions.RemoveEmptyEntries);

            string pattern = @"^:(\w+)";
            Match m = Regex.Match(headParts[0], pattern);
            string sender = m.Groups[1].Value.Trim();
            string target = headParts[1].Trim();

            if (sender == target)
            {
                return;
            }

            Channel c = target.StartsWith("#") ? new Channel(target) : null;
            User s = new User(sender);
            User t = c == null ? new User(target) : null;

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

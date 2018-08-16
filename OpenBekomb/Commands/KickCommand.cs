using plib.Util;

namespace OpenBekomb.Commands
{
    public class KickCommand : ABotCommand
    {
        public KickCommand(ABot _bot) 
            : base(_bot)
        {
        }

        public override string Name
        {
            get
            {
                return "KICK";
            }
        }

        public override void Answer(string _sender, string _target, string _messageBody)
        {
            //string[] headParts = _messageHead.Split(new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] targetParts = _target.Split(' ');
            string kickedPerson = targetParts[1];
            L.Log(kickedPerson + "was kicked");
            if (kickedPerson == Owner.m_Config.m_Name)
            {
                Channel c = Owner.GetChannel(targetParts[0]);
                Owner.HasParted(c);
            }
        }
    }
}

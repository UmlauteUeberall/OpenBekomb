using System.Linq;

namespace OpenBekomb.Modules
{
    public class CIModule : AModule
    {
        public CIModule(ABot _bot) : base(_bot)
        {
        }

        public override string Name
        {
            get
            {
                return "ci";
            }
        }

        public override void Answer(Channel _chan, User _sender, User _target, string _message)
        {

            base.Answer(_chan, _sender, _target, _message);

            string[] words = _message.Split(' ');
            if (words.Length < 2 || words[0] != Owner.m_Config.m_Symbol)
            {
                return;
            }

            string command = $"set($SENDER,{_chan?.ToString() ?? _sender.ToString()})" +
                string.Join(" ", words.Skip(1).ToArray());

            Owner.AddCICommand(command);
        }
    }
}
namespace OpenBekomb.Commands
{
    class RenameFailedCommand : ABotCommand
    {
        public override string Name => "433";


        public RenameFailedCommand(ABot _bot) 
            : base(_bot)
        {
        }


        public override void Answer(string _sender, string _target, string _messageBody)
        {
            if (_messageBody.EndsWith("Nickname is already in use."))
            {
                string name = Owner.Mod<Modules.RenameModule>().m_OldName;
                Owner.SendRawMessage($"NICK {name}");
                Owner.m_Name += name;
            }
        }
    }
}

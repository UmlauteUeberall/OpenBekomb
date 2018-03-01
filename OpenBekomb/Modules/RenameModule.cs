namespace OpenBekomb.Modules
{
    class RenameModule : AModule
    {
        public override string Name => "rename";

        private float m_timer = CHECK_TIME;
        public const float CHECK_TIME = 30;
        public string m_OldName;

        public RenameModule(ABot _bot) 
            : base(_bot)
        {
            m_OldName = _bot.m_Name;
        }

        public override void Update(float _deltaTime)
        {
            if (Owner.m_Name == Owner.m_Config.m_Name)
                return;

            m_timer -= _deltaTime;
            if (m_timer <= 0)
            {
                m_OldName = Owner.m_Name;
                Owner.SendRawMessage($"NICK {Owner.m_Config.m_Name}");
                Owner.m_Name = Owner.m_Config.m_Name;
                //Owner.SendRawMessage($"PING :{System.DateTime.Now.Ticks}");
                m_timer = CHECK_TIME;
            }
        }

    }
}

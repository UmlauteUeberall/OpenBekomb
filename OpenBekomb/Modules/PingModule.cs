
namespace OpenBekomb.Modules
{
    public class PingModule : AModule
    {
        private float m_timer = MAX_PINGTIME;

        //public float LastPingTime;

        public const float MAX_PINGTIME = 30;

        public PingModule(ABot _bot) 
            : base(_bot)
        {
        }

        public override string Name
        {
            get
            {
                return "ping";
            }
        }

        public override void Update(float _deltaTime)
        {
            m_timer -= _deltaTime;
            if (m_timer <= 0)
            {
                Owner.SendRawMessage($"PING :{System.DateTime.Now.Ticks}");
                m_timer = MAX_PINGTIME;
            }
        }

        //public override void Answer(Channel _chan, User _sender, User _target, string _message)
        //{
        //    
        //}


    }
}

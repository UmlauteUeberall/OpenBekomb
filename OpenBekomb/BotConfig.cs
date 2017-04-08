using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBekomb
{
    public class BotConfig
    {
        public string[] m_StartChannels;
        public string m_Name;
        public string m_FullName;
        public static BotConfig Default { get { return s_default; } }
        private static BotConfig s_default = new BotConfig() { m_StartChannels = new string[0], m_Name = "openBekomb", m_FullName = "openBekomb default" };
    }
}

﻿using CommandInterpreter;
namespace OpenBekomb
{
    public class BotConfig
    {
        public string[] m_StartChannels;
        public string m_Name;
        public string m_FullName;
        public string m_Symbol;
        public ACommand[] m_CICommands;
        public ACommand[] m_BlackListedCICommands;
        public string[] m_StartCICommands;

        public static BotConfig Default { get { return s_default; } }

        private static BotConfig s_default = new BotConfig()
        {
            m_StartChannels = new string[0],
            m_Name = "openBekomb",
            m_FullName = "openBekomb default",
            m_Symbol = "☃",
            m_CICommands = null
        };
    }
}

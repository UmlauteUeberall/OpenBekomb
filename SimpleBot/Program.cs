using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBekomb;
using SimpleBot.CICommands;
using CommandInterpreter.Commands;
using CommandInterpreter;
using CommandInterpreter.Calculator;

namespace SimpleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleBot b = new SimpleBot("irc.euirc.org", 6667);

            b.Run(new BotConfig() {
                                    m_Name = "simpleBot",
                                    m_FullName = "simpleBot made with openBekomb",
                                    m_StartChannels = new [] 
                                    {
                                                "#durp",
                                                "#hurp",
                                                "#/prog/bot"
                                    },
                                    m_Symbol = "ü",
                                    m_CICommands = new ACommand[] 
                                    {
                                        new CIDDateCommand(),
                                        new CICronCommand(),
                                        new Calc()
                                    },
                                    m_BlackListedCICommands = new ACommand[] 
                                    {
                                        new WriteCommand(),
                                        new CommandCommand(),
                                        new LoadCommand(),
                                        new LogCommand(),
                                        new SaveVCommand(),
                                        new LoadVCommand(),
                                    },
                                    m_StartCICommands = new[]
                                    {
                                        "cron(0h,0m,run(#/prog/bot,ddate()))"
                                    }
                                });
        }
    }
}

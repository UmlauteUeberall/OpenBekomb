﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBekomb;
using CommandInterpreter;
using CommandInterpreter.Commands;
using Casio.CICommands;

namespace Casio
{
    class Program
    {
        static void Main(string[] args)
        {
            Casio b = new Casio("irc.euirc.org", 6667);

            b.Run(new BotConfig()
            {
                m_Name = "casio",
                m_FullName = "casio made with openBekomb",
                m_StartChannels = new string[]
                                    {
                                                //"#kohlchan"
                                    },
                m_Symbol = "c",
                m_CICommands = new ACommand[]
                                    {
                                        new CIVSCommand(),
                                        new CISearchCommand(),
                                        new CIFileCommand(),
                                        new CITextCommand()
                                    },
                m_BlackListedCICommands = new ACommand[]
                                    {
                                        new WriteCommand(),
                                        new CommandCommand(),
                                        new LoadCommand(),
                                        new LogCommand(),
                                        new SaveVCommand(),
                                        new LoadVCommand(),
                                    }
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using plib.Util;
using System.Text.RegularExpressions;
using OpenBekomb.Commands;
using OpenBekomb.Modules;
using CommandInterpreter;
using OpenBekomb.CICommands;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace OpenBekomb
{
    public abstract class ABot
    {
        public static ABot Bot { get; private set; }

        public BotConfig m_Config { get; private set; }
        public bool Started { get; private set; }

        public const float TARGET_FPS = 60;
        public const long MAX_MILLISEC_PER_FRAME = (long)(1 / TARGET_FPS * 1000);

        private bool m_isRunning;
        private Socket m_socket;

        private Thread m_MessageThread;
        private Thread m_ConsoleInputThread;

        private List<AModule> m_modules = new List<AModule>();
        private List<ABotCommand> m_commands = new List<ABotCommand>();
        private List<Channel> m_channels = new List<Channel>();

        public ABot(string _host, int _port)
        {
            Bot = this;

            m_isRunning = true;

            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.Connect(_host, _port);
            m_MessageThread = new Thread(ProcessInput);
            m_MessageThread.Start();
            m_ConsoleInputThread = new Thread(ConsoleInput);
            m_ConsoleInputThread.Start();

            m_commands.Add(new PingCommand(this));
            m_commands.Add(new PrivMsgCommand(this));
            
        }

        public void Run(BotConfig _config = null)
        {
            m_Config = _config ?? BotConfig.Default; 
            SendRawMessage($"NICK {m_Config.m_Name}");
            SendRawMessage($"USER {m_Config.m_Name} biep311.de {m_Config.m_FullName} :{m_Config.m_Name}");
            // Warten auf Ende der MOTD
            while (!Started)
            {
                Thread.Sleep(100);
            }

            foreach (string cs in m_Config.m_StartChannels)
            {
                Join(cs);
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            long time = sw.ElapsedMilliseconds;
            try
            {
                while (m_isRunning)
                {
                    Update((sw.ElapsedMilliseconds - time) / 1000.0f);
                    time = sw.ElapsedMilliseconds;
                    Thread.Sleep((int)(Math.Max(MAX_MILLISEC_PER_FRAME - sw.ElapsedMilliseconds + time, 0)));
                }
            }
            catch (Exception _ex)
            {
                L.LogW(_ex);
            }
            finally
            {
                m_MessageThread.Abort();
                m_MessageThread = null;
            }
        }

        protected virtual void Update(float _deltaTime)
        {
            foreach (AModule mod in m_modules)
            {
                mod.Update(_deltaTime);
            }
        }


        protected virtual void ProcessInput()
        {
            Queue<string> incommingMessages = new Queue<string>();
            byte[] buffer = new byte[1024];
            string message;
            string currentLine;
            //string[] messageParts;
            ABotCommand mod;
            Match m;
            #region Alarm
            // group 1 complete header
            // group 2 command name
            // group 3 body
            #endregion
            string pattern = $@"^(:?[^:]*({
                string.Join("|", 
                            m_commands.Select(o => o.Name).ToArray())
                            })[^:]*):(.*)";


            // Erste Nachricht ist ein Ping das beantwortet werden muss
            message = ReceiveMessage();
            
            m = Regex.Match(message.Split(new[] { "\r\n" }, 
                        StringSplitOptions.RemoveEmptyEntries)[1], pattern);
            m_commands.FirstOrDefault(o => o is PingCommand).Answer(m.Groups[1].Value, m.Groups[3].Value);

            // Warte auf das Ende der MOTD
            while (!Started)
            {
                message = ReceiveMessage();
                message = message.Replace("\0", "");
                message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ForEach(o => incommingMessages.Enqueue(o));
                while (incommingMessages.Count > 0)
                {
                    currentLine = incommingMessages.Dequeue();
                    if (Regex.IsMatch(currentLine, @"^:\S* 376"))
                    {
                        Started = true;
                        break;
                    }
                }
            }

            while (true)
            {
                message = ReceiveMessage();
                message = message.Replace("\0", "");
                message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).
                    ForEach(o => incommingMessages.Enqueue(o));


                while (incommingMessages.Count > 0)
                {
                    currentLine = incommingMessages.Dequeue();
                    //messageParts = currentLine.Split(' ');
                    m = Regex.Match(currentLine, pattern);
                    mod = m_commands.FirstOrDefault(o => o.Name == m.Groups[2].Value);
                    if (mod != null)
                    {
                        mod.Answer(m.Groups[1].Value, m.Groups[3].Value);  
                    }
                }
            }
        }

        private string ReceiveMessage()
        {
            byte[] buffer = new byte[1024];
            int length = 0;
            string message = "";
            do
            {
                length = m_socket.Receive(buffer);
                if (length == 0)    // Disconnect
                {
                    ShutDown();
                    return "";
                }
                message += Encoding.UTF8.GetString(buffer);
                Array.Clear(buffer, 0, buffer.Length);
            } while (length == 1024);
            L.Log(message);

            return message;
        }

        private void ConsoleInput()
        {
            CmdInterpreter ci = new CmdInterpreter();
            ci.LoadCoreUtils();
            ci.AddProgram<CISendCommand>();
            ci.AddProgram<CIJoinCommand>();
            ci.Initialize(Console.ReadLine, L.Log, L.LogE);
            while (true)
            {
                ci.Run();
            }
        }

        public void Join(Channel _channel)
        {
            Join(_channel.Name);
        }

        public void Join(string _channelName)
        {
            Channel c = new Channel(_channelName);
            SendRawMessage($"JOIN {_channelName}");

            m_channels.Add(c);
        }

        public void SendMessage(User _user, string _message)
        {
            SendMessage(_user.Name, _message);
        }

        public void SendMessage(string _user, string _message)
        {
            SendRawMessage($"PRIVMSG {_user} :{_message}");
        }

        public void SendRawMessage(string _text)
        {
            m_socket.Send(Encoding.Default.GetBytes(_text + "\r\n"));
        }

        public void ShutDown()
        {
            m_isRunning = true;
        }

        protected virtual void CleanUp()
        {

        }
    }
}

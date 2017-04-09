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
        private Thread m_CIThread;

        //private List<AModule> m_modules = new List<AModule>();
        //private List<ABotCommand> m_commands = new List<ABotCommand>();
        protected List<Channel> m_channels = new List<Channel>();


        private Dictionary<Type, AModule> m_modules = new Dictionary<Type, AModule>();
        private Dictionary<Type, ABotCommand> m_commands = new Dictionary<Type, ABotCommand>();

        private Queue<string> m_pendingCICommands = new Queue<string>();
        private object m_ciCommandsLock = new object();


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
            m_CIThread = new Thread(CILoop);
            m_CIThread.Start();

            AddCommand(new PingCommand(this));
            AddCommand(new PrivMsgCommand(this));
            AddCommand(new PongCommand(this));

            AddModule(new PingModule(this));
            AddModule(new CIModule(this));
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
            foreach (var mod in m_modules)
            {
                mod.Value.Update(_deltaTime);
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
                            m_commands.Select(o => o.Value.Name).ToArray())
                            })[^:]*):(.*)";


            // Erste Nachricht ist ein Ping das beantwortet werden muss
            message = ReceiveMessage();
            
            m = Regex.Match(message.Split(new[] { "\r\n" }, 
                        StringSplitOptions.RemoveEmptyEntries)[1], pattern);
            Com<PingCommand>().Answer(m.Groups[1].Value, m.Groups[3].Value);

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
                    mod = Com(m.Groups[2].Value);
                    if (mod != null)
                    {
                        mod.Answer(m.Groups[1].Value, m.Groups[3].Value);  
                    }
                }
            }
        }

        public void ProcessModulesAnswers(Channel _chan, User _sender, User _target, string _message)
        {
            foreach (var m in m_modules)
            {
                m.Value.Answer(_chan, _sender, _target, _message);
            }
        }

        private string ReceiveMessage()
        {
            byte[] buffer = new byte[1024];
            int length = 0;
            string message = "";
            do
            {
                try
                {
                    length = m_socket.Receive(buffer);
                    if (length == 0)    // Disconnect
                    {
                        throw new SocketException();
                    }
                    message += Encoding.UTF8.GetString(buffer);
                    Array.Clear(buffer, 0, buffer.Length);
                }
                catch (SocketException _ex)
                {
                    ShutDown();
                    return "";
                }
            } while (length == 1024);
            L.Log(message);

            return message;
        }

        public T Mod<T>() where T: AModule
        {
            if (!m_modules.ContainsKey(typeof(T)))
            {
                return null;
            }
            return (T) m_modules[typeof(T)];
        }

        public AModule Mod(string _name)
        {
            return m_modules.FirstOrDefault(o => o.Value.Name == _name).Value;
        }

        public T Com<T>() where T : ABotCommand
        {
            if (!m_commands.ContainsKey(typeof(T)))
            {
                return null;
            }
            return (T)m_commands[typeof(T)];
        }

        public ABotCommand Com(string _name)
        {
            return m_commands.FirstOrDefault(o => o.Value.Name == _name).Value;
        }

        public void AddModule(AModule _mod)
        {
            m_modules.Add(_mod.GetType(), _mod);
        }

        public void AddCommand(ABotCommand _cmd)
        {
            m_commands.Add(_cmd.GetType(), _cmd);
        }

        public void RemoveModule(AModule _mod)
        {
            m_modules.Remove(_mod.GetType());
        }

        public void RemoveCommand(ABotCommand _cmd)
        {
            m_commands.Remove(_cmd.GetType());
        }

        public void AddCICommand(string _ciCode)
        {
            lock (m_ciCommandsLock)
            {
                m_pendingCICommands.Enqueue(_ciCode);
            }
        }

        private void ConsoleInput()
        {
            
            while (true)
            {
                string s = Console.ReadLine();
                AddCICommand(s);
            }
        }

        private void CILoop()
        {
            CmdInterpreter ci = new CmdInterpreter();
            ci.LoadCoreUtils();
            ci.AddProgram<CommandInterpreter.Calculator.Calc>();
            ci.AddProgram<CISendCommand>();
            ci.AddProgram<CIJoinCommand>();
            ci.Initialize(L.Log, L.LogE);

            string currentCMD;

            while (true)
            {
                lock (m_ciCommandsLock)
                {
                    if (m_pendingCICommands.Count == 0)
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                    currentCMD = m_pendingCICommands.Dequeue();

                    ci.Run(currentCMD);
                }
            }
        }

        public bool IsAdressed(string _message)
        {
            return _message.StartsWith($"{m_Config.m_Name}:")
                || _message.StartsWith(m_Config.m_Symbol);
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

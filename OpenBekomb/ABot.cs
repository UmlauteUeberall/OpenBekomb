using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using plib.Util;
using CommandInterpreter;
using OpenBekomb.Commands;
using OpenBekomb.Modules;
using OpenBekomb.CICommands;

using Stopwatch = System.Diagnostics.Stopwatch;
using Type = System.Type;
using StringSplitOptions = System.StringSplitOptions;

namespace OpenBekomb
{
    public abstract class ABot
    {
        public static ABot Bot { get; private set; }

        public BotConfig m_Config { get; private set; }
        public bool Started { get; private set; }
        // false setzen um den MainThread zum Reconnect zu bringen
        public bool IsConnected { get; private set; }

        public string m_Name;

        public const float TARGET_FPS = 60;
        public const long MAX_MILLISEC_PER_FRAME = (long)(1 / TARGET_FPS * 1000);

        private bool m_isRunning;

        private Socket m_socket;

        private Thread m_MessageThread;
        private Thread m_ConsoleInputThread;
        private Thread m_CIThread;

        protected List<Channel> m_channels = new List<Channel>();
        protected List<User> mo_users = new List<User>();


        private Dictionary<Type, AModule> m_modules = new Dictionary<Type, AModule>();
        private Dictionary<Type, ABotCommand> m_commands = new Dictionary<Type, ABotCommand>();

        private Queue<string> m_pendingCICommands = new Queue<string>();
        private object m_ciCommandsLock = new object();
        private string m_host;
        private int m_port;

        public ABot(string _host, int _port)
        {
            Bot = this;
            m_host = _host;
            m_port = _port;

            m_isRunning = true;

            L.APP_NAME = GetType().Name;

            // Socket
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.Connect(_host, _port);

            //Commands
            AddCommand(new PingCommand(this));
            AddCommand(new PrivMsgCommand(this));
            AddCommand(new PongCommand(this));
            AddCommand(new KickCommand(this));
            AddCommand(new RenameFailedCommand(this));
            AddCommand(new JoinCommand(this));
            AddCommand(new NicklistRecievedCommand(this));
            AddCommand(new PartCommand(this));

            //Modules
            AddModule(new PingModule(this));
            AddModule(new CIModule(this));
            AddModule(new RenameModule(this));


            //Threads
            m_MessageThread = new Thread(ProcessInput);
            m_MessageThread.Name = "Message-Thread";
            m_MessageThread.Start();
            m_ConsoleInputThread = new Thread(ConsoleInput);
            m_ConsoleInputThread.Name = "ConsoleInput-Thread";
            m_ConsoleInputThread.Start();
            m_CIThread = new Thread(CILoop);
            m_CIThread.Name = "CI-Thread";
            m_CIThread.Start();


        }

        public void Run(BotConfig _config = null)
        {
            m_Config = _config ?? BotConfig.Default;
            m_Name = m_Config.m_Name;
            SendRawMessage($"NICK {m_Name}");
            SendRawMessage($"USER {m_Name} biep311.de {m_Config.m_FullName} :{m_Name}");
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
            while (m_isRunning)
            {
                try
                {
                    while (m_isRunning)
                    {
                        if (!IsConnected)
                        {
                            throw new System.Exception();
                        }

                        Update((sw.ElapsedMilliseconds - time) / 1000.0f);
                        time = sw.ElapsedMilliseconds;
                        Thread.Sleep((int)(System.Math.Max(MAX_MILLISEC_PER_FRAME - sw.ElapsedMilliseconds + time, 0)));
                    }
                }
                catch (System.Exception _ex)
                {
                    L.LogW(_ex);
                    Restart();
                }
            }

            m_MessageThread.Abort();
            m_MessageThread = null;
            m_CIThread.Abort();
            m_CIThread = null;

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
            ABotCommand com;
            Match m;
            #region Alarm
            // group 1 complete header
            // group 2 command name
            // group 3 body
            #endregion
            //string pattern = $@"^(:?[^:]*({
            //    string.Join("|", 
            //                m_commands.Select(o => o.Value.Name).ToArray())
            //                })[^:]*):(.*)";
            //              RINU!
            string pattern = @"^(?:[:](\S+) )?(\S+)(?: (?!:)(.+?))?(?: [:](.+))?$";


            // Erste Nachricht ist ein Ping das beantwortet werden muss
            message = ReceiveMessage();

            if (string.IsNullOrEmpty(message))
            {
                throw new System.ArgumentException("message is null");
            }

            while (message.EndsWith(m_Name + " :Nickname is already in use."))
            {
                SendRawMessage($"NICK {m_Name + "_"}");
                m_Name += "_";

                message = ReceiveMessage();
            }

            try
            {

                m = Regex.Match(message.Split(new[] { "\r\n" },
                            StringSplitOptions.RemoveEmptyEntries)[1], pattern);
                Com<PingCommand>().Answer(m.Groups[1].Value, m.Groups[3].Value, m.Groups[4].Value);
            }
            catch (System.Exception _ex)
            {
                L.LogE(_ex);
                return;
            }

            // Warte auf das Ende der MOTD
            while (!Started)
            {
                message = ReceiveMessage();
                
                message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ForEach(o => incommingMessages.Enqueue(o));
                while (incommingMessages.Count > 0)
                {
                    currentLine = incommingMessages.Dequeue();
                    if (Regex.IsMatch(currentLine, @"^:\S* 376"))
                    {
                        Started = true;
                        IsConnected = true;
                        break;
                    }
                }
            }

            while (true)
            {
                message = ReceiveMessage();
                message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).
                    ForEach(o => incommingMessages.Enqueue(o));


                while (incommingMessages.Count > 0)
                {
                    currentLine = incommingMessages.Dequeue();

                    if (currentLine.EndsWith($"462 {m_Name}: You may not reregister"))
                    {
                        continue;
                    }

                    //messageParts = currentLine.Split(' ');
                    m = Regex.Match(currentLine, pattern);
                    com = Com(m.Groups[2].Value);
                    if (com != null)
                    {
                        com.Answer(m.Groups[1].Value, m.Groups[3].Value, m.Groups[4].Value);
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
                    System.Array.Clear(buffer, 0, buffer.Length);
                }
                catch (SocketException)
                {
                    IsConnected = false;
                    Thread.CurrentThread.Abort();
                    return "";
                }
            } while (length == 1024);
            message = message.Replace("\0", "");
            message = message.Trim();
            L.Log(message);

            return message;
        }

        public T Mod<T>() where T : AModule
        {
            if (!m_modules.ContainsKey(typeof(T)))
            {
                return null;
            }
            return (T)m_modules[typeof(T)];
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
                try
                {

                    string s = System.Console.ReadLine();
                    AddCICommand(s);
                }
                catch (System.Exception _ex)
                {
                    L.LogE(_ex);
                }
            }
        }

        private void CILoop()
        {
            #region -- Init --
            while (m_Config == null)
            {
                Thread.Sleep(100);
            }

            CmdInterpreter ci = new CmdInterpreter();
            ci.LoadCoreUtils();
            ci.AddProgram<CommandInterpreter.Calculator.Calc>();
            ci.AddProgram<CISendCommand>();
            ci.AddProgram<CIJoinCommand>();
            ci.AddProgram<CIPartCommand>();
            ci.AddProgram<CIKickCommand>();
            ci.AddProgram<CIPingCommand>();
            ci.AddProgram<CIRunCommand>();

            ci.AddVariable("$SENDER", "tcl");

            m_Config.m_CICommands?.ForEach(o => ci.AddProgram(o));

            m_Config.m_BlackListedCICommands?.ForEach(o => ci.RemoveProgram(o));

            ci.Initialize(L.Log, L.LogE);


            m_Config.m_StartCICommands?.ForEach(o => ci.Run(o));

            #endregion

            string currentCMD;

            bool shallWait = false;

            while (true)
            {
                try
                {
                    while (!IsConnected)
                    {
                        Thread.Sleep(100);
                    }

                    while (IsConnected)
                    {
                        if (shallWait)
                        {
                            Thread.Sleep(50);
                            shallWait = false;
                        }

                        lock (m_ciCommandsLock)
                        {
                            if (m_pendingCICommands.Count == 0)
                            {
                                shallWait = true;
                                continue;
                            }
                            currentCMD = m_pendingCICommands.Dequeue();

                            ci.Run(currentCMD);
                        }
                    }
                    throw new System.Exception();
                }
                catch (System.Exception _ex)
                {
                    L.LogE(_ex);
                    IsConnected = false;
                }
            }
        }

        public bool IsAdressed(string _message)
        {
            return _message.StartsWith($"{m_Name}:")
                || _message.StartsWith(m_Config.m_Symbol);
        }

        internal Channel GetChannel(string _channelName)
        {
            return m_channels.FirstOrDefault(o => o.Name == _channelName);
        }

        internal User GetUser(string _user)
        {
            User u = mo_users.FirstOrDefault(o => o.Name == _user);
            if (u == null)
            {
                u = new User(_user);
                mo_users.Add(u);
            }

            return u;
        }

        public void Join(Channel _channel)
        {
            Join(_channel.Name);
        }

        public void Join(string _channelName)
        {
            //Channel c = new Channel(_channelName);

            if (!m_channels.Any(o => o.Name == _channelName))
            {
                SendRawMessage($"JOIN {_channelName}");
                SendRawMessage($"NAMES {_channelName}");

                //m_channels.Add(c);
            }
        }

        public void Part(Channel _channel, string _message)
        {
            Part(_channel.Name, _message);
        }

        public void Part(string _channelName, string _message)
        {
            Channel c = m_channels.FirstOrDefault(o => o.Name == _channelName);
            if (c != null)
            {
                string s = $"PART {_channelName} :{_message ?? ""}";
                SendRawMessage(s);

                //Parted(c);
            }
        }

        public void HasJoined(Channel _channel)
        {
            if (!m_channels.Contains(_channel))
            {
                m_channels.Add(_channel);
            }
        }
        
        public void HasParted(Channel _channel)
        {
            m_channels.Remove(_channel);
        }

        public void Kick(Channel _channel, User _user, string _reason)
        {
            Kick(_channel.Name, _user.Name, _reason);
        }

        public void Kick(string _channelName, string _userName, string _reason)
        {
            string s = $"KICK {_channelName} {_userName} :{_reason ?? _userName}";
            SendRawMessage(s);
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
            m_socket.Send(Encoding.UTF8.GetBytes(_text + "\r\n"));
        }

        public void Restart()
        {
            L.LogW("Restart");
            Started = false;

            if (m_MessageThread != null)
            {
                m_MessageThread.Abort();
                m_MessageThread = null;     // C Style
            }

            try
            {
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.Connect(m_host, m_port);
            }
            catch (System.Exception _ex)
            {
                L.LogE(_ex);

                Thread.Sleep(2300);
                Restart();
            }

            SendRawMessage($"NICK {m_Name}");
            SendRawMessage($"USER {m_Name} biep311.de {m_Config.m_FullName} :{m_Name}");

            var oldChans = m_channels;
            m_channels = new List<Channel>();

            m_MessageThread = new Thread(ProcessInput);
            m_MessageThread.Name = "Message-Thread";
            m_MessageThread.Start();

            while (!IsConnected)
            {
                if (!m_MessageThread.IsAlive)
                {
                    Thread.Sleep(2300);
                    Restart();              // Dumm weil es auf lange sicht eine Stackoverflowexception geben könnte bei fehlender Verbindung über lange Zeit
                    m_channels = oldChans;  // Um die alten Kanäle über mehrere Neustarts zu retten, sieht derpy aus
                    return;
                }

                Thread.Sleep(100);
            }

            oldChans.ForEach(o => Join(o.Name));
        }

        public void ShutDown()
        {
            m_isRunning = false;
        }

        protected virtual void CleanUp()
        {

        }
    }
}

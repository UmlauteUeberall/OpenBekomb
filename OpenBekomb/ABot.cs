using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Stopwatch = System.Diagnostics.Stopwatch;
using plib.Util;

namespace OpenBekomb
{
    public abstract class ABot
    {

        public abstract string Name { get; }

        public const float TARGET_FPS = 10;
        public const long MAX_MILLISEC_PER_FRAME = (long)(1 / TARGET_FPS * 1000);

        private bool m_isRunning;
        private Socket m_socket;

        private Thread m_MessageThread;
        private List<AModule> m_modules = new List<AModule>();

        public ABot(string _host, int _port)
        {
            m_isRunning = true;

            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.Connect(_host, _port);
            m_MessageThread = new Thread(ProcessInput);
            m_MessageThread.Start();

            m_modules.Add(new PingModule(this));
            SendRawMessage($"NICK {Name}");
            Thread.Sleep(5000);

            SendRawMessage($"USER {Name} biep311.de {Name} :{Name}");
            Thread.Sleep(5000);

            SendRawMessage($"PRIVMSG {"lct"} :Mein Sack");
            SendRawMessage($"JOIN #hurp");
            SendRawMessage($"PRIVMSG #hurp :MEIN SACK");

        }

        public void Run()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long time = sw.ElapsedMilliseconds;
            //long lastTime = ;
            try
            {
                while (m_isRunning)
                {
                    //Console.WriteLine((sw.ElapsedMilliseconds - time));
                    
                    Update((sw.ElapsedMilliseconds - time) / 1000.0f);
                    time = sw.ElapsedMilliseconds;
                    Thread.Sleep((int)(Math.Max(MAX_MILLISEC_PER_FRAME - sw.ElapsedMilliseconds + time,0)));
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex);
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

        private Queue<string> m_incommingMessages = new Queue<string>();

        protected virtual void ProcessInput()
        {
            byte[] buffer = new byte[1024];
            string message;
            string[] messageParts;
            AModule mod;
            while (true)
            {
                m_socket.Receive(buffer);
                message = Encoding.Default.GetString(buffer);
                Console.WriteLine(message);
                message = message.Replace("\0", "");
                message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ForEach(o => m_incommingMessages.Enqueue(o));
                while (m_incommingMessages.Count > 0)
                {
                    messageParts = m_incommingMessages.Dequeue().Split(' ');
                    mod = m_modules.FirstOrDefault(o => o.Message == messageParts[0]);
                    if (mod != null)
                    {
                        mod.Answer(string.Join(" ", messageParts));
                    }
                }
            }
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

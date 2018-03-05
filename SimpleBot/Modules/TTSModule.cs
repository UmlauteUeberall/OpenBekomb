#if !MONO
using System.Speech.Synthesis;
#endif
using OpenBekomb;
using OpenBekomb.Modules;
using System.Threading;
using System.Collections.Generic;
using plib.Util;
using plib.Util.Helper;
using System.Net.Sockets;
using System.Net;

namespace SimpleBot.Modules
{
    public class TTSModule : AModule
    {
        public enum ETTSLanguage
        {
            ENGLISH,
            DEUTSCH
        }

        private readonly Queue<Tuple<ETTSLanguage, string>> m_ttsMessages = new Queue<Tuple<ETTSLanguage, string>>();

        private System.Random m_rand = new System.Random();
        private Thread m_ttsThread;
        private object m_messageLock = new object();
#if MONO
        private TcpListener mi_serverListener;
        private Thread mi_serverListenerThread;
        private object mi_clientListLock = new object();
        private readonly List<TcpClient> mi_clients = new List<TcpClient>();
#endif

        public TTSModule(ABot _bot)
            : base(_bot)
        {
            m_ttsThread = new Thread(DoTTs);
            m_ttsThread.Name = "TTS-Thread";
            m_ttsThread.Start();
#if MONO
            mi_serverListener = new TcpListener(System.Net.IPAddress.Any, 23523);
            mi_serverListener.Start();
            mi_serverListenerThread = new Thread(AcceptListeners);
            mi_serverListenerThread.Name = "Listener-Thread";
            mi_serverListenerThread.Start();
#endif
        }

        public override string Name
        {
            get
            {
                return "TTS";
            }
        }

        public override void Answer(Channel _chan, User _sender, User _target, string _message)
        {
            base.Answer(_chan, _sender, _target, _message);
            string m = _message.Trim();
            if (string.IsNullOrEmpty(m))
            {
                return;
            }

            lock (m_messageLock)
            {
                AddMessage(_message);
            }
        }

        public void AddMessage(string _message, ETTSLanguage _language = ETTSLanguage.ENGLISH)
        {
            m_ttsMessages.Enqueue(new Tuple<ETTSLanguage, string>(_language, _message));
        }

#if MONO
        private void AcceptListeners()
        {
            TcpClient client;
            while (true)
            {
                try
                {

                    client = mi_serverListener.AcceptTcpClient();

                    lock (mi_clientListLock)
                    {
                        mi_clients.Add(client);
                    }
                }
                catch (System.Exception _ex)
                {
                    L.LogE(_ex);
                }
            }
        }

        public string fu_GetLoggedInUsers()
        {
            string s = "";
            lock (mi_clientListLock)
            {
                mi_clients.ForEach(o => s += IPAddress.Parse(((IPEndPoint) o.Client.RemoteEndPoint).Address.ToString()).ToString() + "\n");
            }

            return s.Trim();
        }
#endif

        private void DoTTs()
        {
#if !MONO
            SpeechSynthesizer ss = new SpeechSynthesizer();
            var voices = ss.GetInstalledVoices();
#else
            byte[] bytes;
#endif


            bool shallWait = false;

            Tuple<ETTSLanguage, string> currentMessage;
            while (true)
            {
                if (shallWait)
                {
                    Thread.Sleep(50);
                    shallWait = false;
                }

                lock (m_messageLock)
                {

                    if (m_ttsMessages.Count == 0)
                    {
                        shallWait = true;
                        continue;
                    }
                    currentMessage = m_ttsMessages.Dequeue();

#if !MONO
                    ss.SelectVoice(voices[m_rand.Next(voices.Count - 1)].VoiceInfo.Name);
                    ss.Rate = -5;


                    try
                    {
                        ss.Speak(currentMessage.M1);

                    }
                    catch (System.ArgumentNullException)
                    {

                    }
#else
                    lock (mi_clientListLock)
                    {
                        bytes = System.Text.Encoding.UTF8.GetBytes(currentMessage.M1);
                        for(int i = 0; i < mi_clients.Count; i++)
                        {
                            try
                            {
                                mi_clients[i].GetStream().Write(bytes, 0, bytes.Length);
                            }
                            catch
                            {
                                mi_clients.RemoveAt(i);
                            }
                        }
                    }


                    /*
                    if (currentMessage.M0 == ETTSLanguage.ENGLISH)
                    {
                        System.Diagnostics.Process.Start($"/bin/bash", $"-c \"espeak '{currentMessage.M1.Replace("'", "").Replace("\"", "")}' 2> /dev/null\"");
                    }
                    else if (currentMessage.M0 == ETTSLanguage.DEUTSCH)
                    {
                        System.Diagnostics.Process.Start($"/bin/bash", $"-c \"espeak -s 105 -p 100 -v de '{currentMessage.M1.Replace("'", "").Replace("\"", "")}' 2> /dev/null\"");
                    }
                    */
#endif
                }
            }
        }
    }
}
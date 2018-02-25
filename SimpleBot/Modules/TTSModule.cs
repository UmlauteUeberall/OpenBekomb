#if !MONO
using System.Speech.Synthesis;
#endif
using OpenBekomb;
using OpenBekomb.Modules;
using System.Threading;
using System.Collections.Generic;
using plib.Util;

namespace SimpleBot.Modules
{
    public class TTSModule : AModule
    {
        private Queue<string> m_ttsMessages = new Queue<string>();

        private System.Random m_rand = new System.Random();
        private Thread m_ttsThread;
        private object m_messageLock = new object();

        public TTSModule(ABot _bot) 
            : base(_bot)
        {
            m_ttsThread = new Thread(DoTTs);
            m_ttsThread.Name = "TTS-Thread";
            m_ttsThread.Start();
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

            lock (m_messageLock)
            {
                AddMessage(_message);
            }
        }

        public void AddMessage(string _message)
        {
            m_ttsMessages.Enqueue(_message);
        }

        private void DoTTs()
        {
#if !MONO
            SpeechSynthesizer ss = new SpeechSynthesizer();
            var voices = ss.GetInstalledVoices();
#endif


            bool shallWait = false;

            string currentMessage;
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
                        ss.Speak(currentMessage);

                    }
                    catch (System.ArgumentNullException)
                    {

                    }
#else
                    System.Diagnostics.Process.Start($"/bin/bash", $"-c \"espeak '{currentMessage.Escape("\"\'")}' 2> /dev/null\"");
#endif
                }
            }
        }
    }
}
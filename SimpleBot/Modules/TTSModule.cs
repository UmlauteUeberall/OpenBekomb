using System;
using System.Speech.Synthesis;
using OpenBekomb;
using OpenBekomb.Modules;
using System.Threading;
using System.Collections.Generic;

namespace SimpleBot.Modules
{
    public class TTSModule : AModule
    {
        private Random m_rand = new Random();

        private Thread m_ttsThread;

        public TTSModule(ABot _bot) 
            : base(_bot)
        {
            m_ttsThread = new Thread(DoTTs);
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
                m_ttsMessages.Enqueue(_message);
            }
        }

        private Queue<string> m_ttsMessages = new Queue<string>();
        private object m_messageLock = new object();

        private void DoTTs()
        {
            SpeechSynthesizer ss = new SpeechSynthesizer();
            var voices = ss.GetInstalledVoices();

            


            string currentMessage;
            while (true)
            {
                lock (m_messageLock)
                {
                    if (m_ttsMessages.Count == 0)
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                    currentMessage = m_ttsMessages.Dequeue();

                    ss.SelectVoice(voices[m_rand.Next(voices.Count - 1)].VoiceInfo.Name);
                    ss.Rate = -5;

                    try
                    {
                        ss.Speak(currentMessage);

                    }
                    catch (ArgumentNullException _ex)
                    {

                    }
                }
            }
        }
    }
}

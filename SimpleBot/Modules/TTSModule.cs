#if !MONO
using System.Speech.Synthesis;
#endif
using OpenBekomb;
using OpenBekomb.Modules;
using System.Threading;
using System.Collections.Generic;
using plib.Util;
using plib.Util.Helper;

namespace SimpleBot.Modules
{
    public class TTSModule : AModule
    {
        public enum ETTSLanguage
        {
            ENGLISH,
            DEUTSCH
        }

        private Queue<Tuple<ETTSLanguage, string>> m_ttsMessages = new Queue<Tuple<ETTSLanguage, string>>();

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

        public void AddMessage(string _message, ETTSLanguage _language = ETTSLanguage.ENGLISH)
        {
            m_ttsMessages.Enqueue(new Tuple<ETTSLanguage, string>(_language, _message));
        }

        private void DoTTs()
        {
#if !MONO
            SpeechSynthesizer ss = new SpeechSynthesizer();
            var voices = ss.GetInstalledVoices();
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
                    if (currentMessage.M0 == ETTSLanguage.ENGLISH)
                    {
                        System.Diagnostics.Process.Start($"/bin/bash", $"-c \"espeak '{currentMessage.M1.Replace("'", "").Replace("\"", "")}' 2> /dev/null\"");
                    }
                    else if (currentMessage.M0 == ETTSLanguage.DEUTSCH)
                    {
                        System.Diagnostics.Process.Start($"/bin/bash", $"-c \"espeak -s 105 -p 100 -v de '{currentMessage.M1.Replace("'", "").Replace("\"", "")}' 2> /dev/null\"");
                    }
#endif
                }
            }
        }
    }
}
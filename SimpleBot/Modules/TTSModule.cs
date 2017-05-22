﻿#if !MONO
using System.Speech.Synthesis;
#endif
using OpenBekomb;
using OpenBekomb.Modules;
using System.Threading;
using System.Collections.Generic;

namespace SimpleBot.Modules
{
    public class TTSModule : AModule
    {
        private System.Random m_rand = new System.Random();

        private Thread m_ttsThread;

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
                m_ttsMessages.Enqueue(_message);
            }
        }

        private Queue<string> m_ttsMessages = new Queue<string>();
        private object m_messageLock = new object();

        static int s;

        private void DoTTs()
        {
#if !MONO
            SpeechSynthesizer ss = new SpeechSynthesizer();
            var voices = ss.GetInstalledVoices();
#endif
            


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

#if !MONO
                    ss.SelectVoice(voices[m_rand.Next(voices.Count - 1)].VoiceInfo.Name);
                    ss.Rate = -5;

                    try
                    {
                        ss.Speak(currentMessage);

                    }
                    catch (System.ArgumentNullException _ex)
                    {

                    }
#else
                    //System.Diagnostics.Process.Start($"touch meinSACK{s++}");
                    System.Diagnostics.Process.Start($"/bin/bash", $"-c \"espeak '{currentMessage}' 2> /dev/null\"");
                    //System.Diagnostics.Process.Start($"/usr/bin/espeak", $"\"{currentMessage}\"");
#endif
                }
            }
        }
    }
}
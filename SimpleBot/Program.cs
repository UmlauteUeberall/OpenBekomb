using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenBekomb;

namespace SimpleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleBot b = new SimpleBot("irc.euirc.org", 6667);

            b.Run(new BotConfig() {
                                    m_Name = "simpleBot",
                                    m_FullName = "simpleBot made with openBekomb",
                                    m_StartChannels = new [] 
                                            {
                                                "#durp",
                                                "#hurp"
                                            }
                                    });

            /*
            SpeechSynthesizer ss = new SpeechSynthesizer();
            var hurp = ss.GetInstalledVoices();

            int i = 0;
            foreach (var durp in hurp)
            {
                ss.SelectVoice(durp.VoiceInfo.Name);
                ss.Rate = -5;

                try
                {
                    //ss.Speak("Alarm");
                   // ss.SetOutputToWaveFile($"file{i++}.wav");
                    ss.Speak("Bernd, stell dir vor, du schlägst jemanden so hart, dass er zu einer Tür wird. Du findest dann heraus, dass so ALLE Türen entstehen, und du wirst in einen Mörderclub involviert, der Türen macht. Je stärker man schlägt, desto besser wird die Tür. Es gibt also irgendwelche super starke Mörder die Leute in venezianische Türen und so einen Scheiß schlagen.");

                }
                catch (ArgumentNullException _ex)
                {

                }
            }
            */
        }
    }
}

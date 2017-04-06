using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Synthesis;

namespace SimpleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //SimpleBot b = new SimpleBot("irc.euirc.org", 6667);

            //b.Run();

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
                    //ss.Speak("Hallo mein name ist Bauchsack");
                    ss.SetOutputToWaveFile($"file{i++}.wav");
                    ss.Speak("Bernd, stell dir vor, du schlägst jemanden so hart, dass er zu einer Tür wird. Du findest dann heraus, dass so ALLE Türen entstehen, und du wirst in einen Mörderclub involviert, der Türen macht. Je stärker man schlägt, desto besser wird die Tür. Es gibt also irgendwelche super starke Mörder die Leute in venezianische Türen und so einen Scheiß schlagen.");

                }
                catch (ArgumentNullException _ex)
                {

                }
            }
        }
    }
}

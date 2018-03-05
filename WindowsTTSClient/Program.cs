using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace WindowsTTSClient
{
    class Program
    {
        private static readonly Queue<string> mi_messages = new Queue<string>();
        private static object mi_messageLock = new object();

        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
            client.Connect("biep311.de",23523);
            
            byte[] bytes;
            string message;

            Thread ttsThread = new Thread(DoTTS);
            ttsThread.Name = "TTS-Thread";
            ttsThread.Start();

            while (true)
            {
                bytes = new byte[1024];
                client.GetStream().Read(bytes, 0, bytes.Length);
                message = Encoding.UTF8.GetString(bytes);
                message = message.Replace("\0", "");

                lock (mi_messageLock)
                {
                    mi_messages.Enqueue(message);
                }
            }
        }

        static void DoTTS()
        {
            System.Random rand = new System.Random();
            SpeechSynthesizer ss = new SpeechSynthesizer();
            var voices = ss.GetInstalledVoices();
            string message;

            while (true)
            {
                ss.SelectVoice(voices[1].VoiceInfo.Name);
                ss.Rate = -5;

                lock (mi_messageLock)
                {
                    if (mi_messages.Count == 0)
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                    message = mi_messages.Dequeue();
                }

                try
                {
                    ss.Speak(message);

                }
                catch (System.ArgumentNullException)
                {

                }
            }
        }
    }
}

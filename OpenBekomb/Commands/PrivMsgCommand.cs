using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Speech.Synthesis;

namespace OpenBekomb.Commands
{
    class PrivMsgCommand : ABotCommand
    {
        public PrivMsgCommand(ABot _bot) 
            : base(_bot)
        {
        }

        public override string Name
        {
            get
            {
                return "PRIVMSG";
            }
        }

        private Random m_rand = new Random();

        public override void Answer(string _messageHead, string _messageBody)
        {
            string[] headParts = _messageHead.Split(new[] { "PRIVMSG" }, StringSplitOptions.RemoveEmptyEntries);

            string pattern = @"^:(\w+)";
            Match m = Regex.Match(headParts[0], pattern);
            string sender = m.Groups[1].Value.Trim();
            string target = headParts[1].Trim();

            if (sender == target)
            {
                return;
            }




            SpeechSynthesizer ss = new SpeechSynthesizer();
            var voices = ss.GetInstalledVoices();

            int i = 0;
            ss.SelectVoice(voices[m_rand.Next(voices.Count - 1)].VoiceInfo.Name);
            ss.Rate = -5;

            try
            {
                //ss.Speak("Alarm");
                // ss.SetOutputToWaveFile($"file{i++}.wav");
                ss.Speak(_messageBody);

            }
            catch (ArgumentNullException _ex)
            {

            }
            







            //if (target.StartsWith("#"))
            //{
            //    Owner.SendMessage(target, _messageBody.Trim());
            //}
            //else
            //{
            //    Owner.SendMessage(sender, _messageBody.Trim());
            //}
        }
    }
}

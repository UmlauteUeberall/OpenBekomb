#if MONO
using CommandInterpreter;
using OpenBekomb;
using SimpleBot.Modules;

namespace SimpleBot.CICommands
{
    [Command("tts-listeners")]
    public sealed class CTTSListenersCommand : ACommand
    {
        public override string ManPage => "tts-listeners prints all logged in ips for tts over http";

        [Runnable]
        public void RunCommand()
        {
            TTSModule ttsMod = ABot.Bot.Mod<TTSModule>();
            m_owner.InvokeOutput(ttsMod.fu_GetLoggedInUsers());
        }
    }
}
#endif
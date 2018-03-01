using CommandInterpreter;

namespace OpenBekomb.CICommands
{
    [Command("ping")]
    public sealed class CIPingCommand : ACommand
    {
        public override string ManPage =>
@"Shows latency to the server
takes no parameters";

        [Runnable]
        public void RunCommand()
        {
            ABot.Bot.SendRawMessage($"PING :{System.DateTime.Now.Ticks}");
        }
    }
}

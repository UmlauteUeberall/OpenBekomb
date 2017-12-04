using CommandInterpreter;

namespace OpenBekomb.CICommands
{
    [Command("join")]
    public sealed class CIJoinCommand : ACommand
    {
        public CIJoinCommand()
        {
        }

        public override string ManPage => 
@"Joins into an irc channel
Needs one parameter";

        [Runnable]
        public void RunCommand(string _targetChannel)
        {
            ABot.Bot.Join(_targetChannel);
        }
    }
}

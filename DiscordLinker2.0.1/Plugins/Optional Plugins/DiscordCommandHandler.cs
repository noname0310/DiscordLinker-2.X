using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("DiscordCommandHandler", "noname", "0.1.0")]
    [Description("Analyzes and processes messages from Discord")]
    class DiscordCommandHandler : CovalencePlugin
    {
        [PluginReference] Plugin DiscordLinker, PlayerList, TimeOfDay;

        private const string F_COMMAND = "command";

        object OnDiscordCommand(string username, string msg, string command, string[] args)
        {
            if (command == "help" || command == "ㅗ디ㅔ")
            {
                DiscordLinker?.Call("IPCEnqueue",
                           "```css\n---------------[명령어 리스트]---------------" +
                           "\nhelp - 명령어 리스트를 봅니다" +
                           "\n" +
                           "\nwho, w, players - 서버에 있는 유저들을 확인합니다" +
                           "\n" +
                           "\ntime, t - 게임시간을 확인합니다 (시:분:초)" +
                           "\n" +
                           "\n*명령어들을 키보드 자판에 맞게 한글로쳐도 작동합니다```"
                           , F_COMMAND);
                return true;
            }
            else if (command == "who" || command == "w" || command == "players" || command == "좨" || command == "ㅈ" || command == "ㅔㅣ묟ㄱㄴ")
            {
                DiscordLinker?.Call("IPCEnqueue",
                           string.Format("```{0}```", (string)PlayerList.Call("PlayersListAPI"))
                           , F_COMMAND);
                return true;
            }
            else if (command == "time" || command == "t" || command == "샤ㅡㄷ" || command == "ㅅ")
            {
                DiscordLinker?.Call("IPCEnqueue",
                           string.Format("```{0}```", "현재 게임시각: " + (string)TimeOfDay.Call("GetTime"))
                           , F_COMMAND);
                return true;
            }
            return null;
        }
    }
}
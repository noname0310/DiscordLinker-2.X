using System;
using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("DiscordConnection", "noname", "1.0.4")]
    [Description("Send ConnectionMsg To Discord")]
    class DiscordConnection : RustPlugin
    {
        private const string F_JOINLEAVE = "joinleave";

        [PluginReference]
        Plugin PlayerList, DiscordLinker;

        private new void LoadDefaultConfig()
        {
            PrintWarning("Creating a new configuration file");
            Config.Clear();

            Config["ConnectMessage"] = "{0}님이 서버에 입장하셨습니다";
            Config["DisconnectMessage"] = "{0}님이 서버에서 퇴장하셨습니다(사유:{1})";
            Config["PlayerListMessage"] = "현재 {0}명이 접속중입니다\n{1}";
            Config["PlayerListIsNullMessage"] = "현재 서버에 아무도 없습니다";
            Config["SendPlayerList"] = true;
            SaveConfig();
        }

        private void OnPlayerConnected(BasePlayer player)
        {
            if (Convert.ToBoolean(Config["SendPlayerList"]) == true)
            {
                string FullMsg = string.Format(Config["ConnectMessage"].ToString(), player.displayName) + "\n\n" + (string)PlayerList.Call("PlayersListAPI");
                DiscordLinker?.Call("IPCEnqueue", string.Format("```{0}```", FullMsg), F_JOINLEAVE);
            }
            else
            {
                DiscordLinker?.Call("IPCEnqueue", string.Format("```{0}```", string.Format(Config["ConnectMessage"].ToString()), player.displayName), F_JOINLEAVE);
            }
        }

        private void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            if (Convert.ToBoolean(Config["SendPlayerList"]) == true)
            {
                string FullMsg = string.Format(Config["DisconnectMessage"].ToString(), player.displayName, reason) + "\n\n" + (string)PlayerList.Call("PlayersListAPI");
                DiscordLinker?.Call("IPCEnqueue", string.Format("```{0}```", FullMsg), F_JOINLEAVE);
            }
            else
            {
                DiscordLinker?.Call("IPCEnqueue", string.Format("```{0}```", string.Format(Config["DisconnectMessage"].ToString(), player.displayName, reason)), F_JOINLEAVE);
            }
        }
    }
}
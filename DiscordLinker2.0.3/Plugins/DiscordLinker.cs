﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("DiscordLinker", "noname", "2.2.0")]
    [Description("Link Between Discord and Rust")]
    class DiscordLinker : CovalencePlugin
    {
        [PluginReference]
        Plugin SocketIPCManager, BetterChatMute, HangulInput, ConvertKor;

        private const string F_LINKERINIT = "linkerinit_r";
        private const string F_COMMAND = "command";
        private const string F_CHAT_READ = "chat_r";
        private const string F_CHAT_SEND = "chat_s";
        private const string F_TEAMCHAT_READ = "teamchat_r";
        private const string F_CONSOLE_READ = "console_r";
        private const string F_CONSOLE_SEND = "console_s";

        #region Hooks

        private new void LoadDefaultConfig()
        {
            PrintWarning("Creating a new configuration file");

            Config.WriteObject(GetDefaultConfig(), true);
        }

        private void Init()
        {
            LoadConfig();
            Application.logMessageReceived += HandleLog;
        }

        private void Unload()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void OnServerInitialized()
        {
            IPCEnqueue("@everyone```서버가 온라인 입니다\n서버측 디스코드 링커가 켜졌습니다```", F_LINKERINIT);
        }

        private object OnPlayerChat(BasePlayer bplayer, string message, ConVar.Chat.ChatChannel chatchannel)
        {
            IPlayer player = bplayer?.IPlayer;
            if (player == null)
                return null;

            switch (chatchannel.ToString())
            {
                case "Global":
                    if (BetterChatMute != null)
                    {
                        bool? muted = BetterChatMute?.Call<bool>("API_IsMuted", player);
                        if (muted.HasValue && muted.Value)
                            break;
                    }

                    if (ConvertKor != null && (bool)ConvertKor.Call("isConvertKor", bplayer)) message = (string)ConvertKor.Call("GetConvertKor", message);
                    else if (HangulInput != null) message = (string)HangulInput.Call("GetConvertedString", player.Id, message);
                    IPCEnqueue(string.Format("**{0}**: {1}", bplayer.displayName, StripHTML(message)), F_CHAT_READ);
                    break;

                case "Team":
                    if (ConvertKor != null && (bool)ConvertKor.Call("isConvertKor", bplayer)) message = (string)ConvertKor?.Call("GetConvertKor", message);
                    else if (HangulInput != null) message = (string)HangulInput?.Call("GetConvertedString", player.Id, message);
                    IPCEnqueue(string.Format("**[{0}] {1}**: {2}", bplayer.currentTeam, bplayer.displayName, StripHTML(message)), F_TEAMCHAT_READ);
                    break;

                default:
                    break;
            }

            return null;
        }

        private void OnIPCReceivedData(JArray jArray)
        {
            LinkerRequestObject[] linkerRequestObjects = jArray.ToObject<LinkerRequestObject[]>();

            foreach (var linkerRequestObject in linkerRequestObjects)
            {
                if (linkerRequestObject.Guild != config.GuildID)
                    continue;

                foreach (var item in config.ChatChannels)
                {
                    if (item.ChannelID != linkerRequestObject.ChannelId)
                        continue;

                    if (item.Filters.Contains(F_COMMAND))
                    {
                        if (linkerRequestObject.Msg.Length != 0 && linkerRequestObject.Msg[0] == config.CommandPrefix)
                        {
                            ParsedArgs parsedmsg = ParseArgs(linkerRequestObject.Msg.Substring(1));

                            if (parsedmsg.Command == "")
                            {
                                IPCEnqueue(string.Format("```\'{0}\'는 디스코드 링커 명령어 키워드입니다\n이 뒤에 구문을 붙임으로서 서버측에서 명령어를 수행하며 응답합니다\n\n명령어 리스트를 보려면 {0}help를 입력하세요```", config.CommandPrefix), F_COMMAND);

                                continue;
                            }

                            object result = Interface.CallHook("OnDiscordCommand", linkerRequestObject.AuthorName, linkerRequestObject.Msg, parsedmsg.Command, parsedmsg.Args);

                            if (result == null)
                            {
                                IPCEnqueue(string.Format("```\"{0}\"는(은) 존재하지 않는 명령어 입니다```", parsedmsg.Command), F_COMMAND);
                            }
                            continue;
                        }
                    }

                    if (item.Filters.Contains(F_CHAT_SEND))
                    {
                        Broadcast(string.Format("<color=#FF9436>[Discord]</color><color=#FFDC7E>{0}</color>: {1}", linkerRequestObject.AuthorName, linkerRequestObject.Msg));
                        Puts(string.Format("{0}: {1}", linkerRequestObject.AuthorName, linkerRequestObject.Msg));
                    }

                    if (item.Filters.Contains(F_CONSOLE_SEND))
                    {
                        covalence.Server.Command(linkerRequestObject.Msg);
                    }
                }
            }
        }
        private void HandleLog(string message, string stackTrace, LogType type)
        {
            if (!string.IsNullOrEmpty(message) && !config.ConsoleFilter.Any(message.Contains))
            {
                IPCEnqueue(message, F_CONSOLE_READ);
            }
        }

        #endregion

        #region ConfigManage

        private PluginConfig config;

        private new void LoadConfig()
        {
            config = Config.ReadObject<PluginConfig>();

            if (config == null)
                config = GetDefaultConfig();

            VersionUpdate(config);
        }

        private void VersionUpdate(PluginConfig config)
        {
            if (config.ConfigVersion < new VersionNumber(2, 1, 0))
            {
                config.ConsoleFilter = GetDefaultConsoleFilter();
            }

            if (config.ConfigVersion < this.Version)
            {
                config.ConfigVersion = this.Version;
                Config.WriteObject(config, true);
                Puts("Config version has been updated");
            }
        }

        private static List<string> GetDefaultConsoleFilter()
        {
            return new List<string>
            {
                "AngryAnt Behave version",
                "alphamapResolution is clamped to the range of",
                "api.facepunch.com/api/public/manifest/",
                "Checking for new Steam Item Definitions..",
                "Floating point textures aren't supported on this device",
                "HDR Render Texture not supported, disabling HDR on reflection probe",
                "Image Effects are not supported on this platform",
                "Loading Prefab Bundle",
                "Missing shader in",
                "Missing projectileID",
                "Motion vectors not supported on a platform that does not support",
                "SwitchParent Missed",
                "saddletest",
                "The image effect Main Camera",
                "The image effect effect -",
                "The referenced script",
                "Unsupported encoding: 'utf8'",
                "Warning, null renderer for ScaleRenderer!",
                "[AmplifyColor]",
                "[AmplifyOcclusion]",
                "[CoverageQueries] Disabled due to unsupported",
                "[CustomProbe]",
                "[Manifest] URI IS",
                "[SpawnHandler] populationCounts",
                ", disk(",
                "Kinematic body only supports Speculative Continuous collision detection"
            };
        }

        private class PluginConfig
        {
            public char CommandPrefix;
            public ulong GuildID;
            public List<ChatChannel> ChatChannels;
            public List<string> ConsoleFilter = GetDefaultConsoleFilter();
            public VersionNumber ConfigVersion;
        }

        public class ChatChannel
        {
            public ulong ChannelID;
            public List<string> Filters;

            public ChatChannel()
            {
                //for json serialize
            }

            public ChatChannel(ulong channelID, List<string> filters)
            {
                ChannelID = channelID;
                Filters = filters;
            }
        }

        private PluginConfig GetDefaultConfig()
        {
            return new PluginConfig
            {
                CommandPrefix = ',',
                GuildID = 0,
                ChatChannels = new List<ChatChannel>()
                {
                    new ChatChannel(
                        0,
                        new List<string>()
                        {
                            F_LINKERINIT,
                            F_COMMAND,
                            F_CHAT_READ,
                            F_CHAT_SEND
                        }
                        ),
                    new ChatChannel(
                        0,
                        new List<string>()
                        {
                            F_CONSOLE_READ,
                            F_CONSOLE_SEND
                        }
                        ),
                    new ChatChannel(
                        0,
                        new List<string>()
                        {
                            F_TEAMCHAT_READ
                        }
                        )
                }
            };
        }

        #endregion

        #region Helper

        private void IPCEnqueue(string str, string filter)
        {
            foreach (var item in config.ChatChannels)
            {
                if (item.Filters.Contains(filter))
                {
                    SocketIPCManager.Call("API_IPCEnqueue", JObject.FromObject(new PluginRequestObject(config.GuildID, item.ChannelID, str)));
                }
            }
        }

        class PluginRequestObject
        {
            public ulong Guild;
            public ulong Channel;
            public string Msg;

            public PluginRequestObject()
            {

            }

            public PluginRequestObject(ulong guild, ulong channel, string msg)
            {
                Guild = guild;
                Channel = channel;
                Msg = msg;
            }
        }

        class LinkerRequestObject
        {
            public ulong Guild;
            public ulong ChannelId;
            public ulong AuthorId;
            public string AuthorName;
            public string Msg;
        }

        private void Broadcast(string msg)
        {
            foreach (var player in players.Connected)
                player.Message(msg);
        }

        private class ParsedArgs
        {
            public string Command;
            public string[] Args;

            public ParsedArgs(string command, string[] args)
            {
                Command = command;
                Args = args;
            }
        }

        private class ParsedArgsList
        {
            public string Command;
            public List<string> Args;

            public ParsedArgsList(string command, List<string> args)
            {
                Command = command;
                Args = args;
            }
        }


        private ParsedArgs ParseArgs(string msg)
        {
            msg = RemoveSpace(msg);
            ParsedArgsList splitArgs = SplitArgs(msg);

            splitArgs.Command = SwitchQuote(splitArgs.Command);
            for (int i = 0; i < splitArgs.Args.Count; i++)
            {
                splitArgs.Args[i] = SwitchQuote(splitArgs.Args[i]);
            }

            return new ParsedArgs(splitArgs.Command, splitArgs.Args.ToArray());
        }

        private string SwitchQuote(string msg)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool PrevIsOSlash = false;

            for (int i = 0; i < msg.Length; i++)
            {
                if (PrevIsOSlash)
                {
                    if (msg[i] == '\"')
                    {
                        stringBuilder.Append('\"');
                    }
                    else
                    {
                        stringBuilder.Append('\\');
                        stringBuilder.Append(msg[i]);
                    }
                }
                else
                {
                    if (msg[i] != '\"' && msg[i] != '\\')
                    {
                        stringBuilder.Append(msg[i]);
                    }
                }

                if (msg[i] == '\\')
                {
                    PrevIsOSlash = true;
                }
                else
                {
                    PrevIsOSlash = false;
                }
            }

            return stringBuilder.ToString();
        }

        private ParsedArgsList SplitArgs(string msg)
        {
            string Head = "";
            List<string> Splitmsg = new List<string>();
            bool IsFirstValue = true;
            bool PrevIsOSlash = false;
            bool FindSpace = true;

            int startindex = 0;
            int endindex = 0;

            for (int i = 0; i < msg.Length; i++)
            {
                if (msg[i] == '\"' && PrevIsOSlash == false)
                {
                    FindSpace = !FindSpace;
                }

                if (msg[i] == '\\')
                {
                    PrevIsOSlash = true;
                }
                else
                {
                    PrevIsOSlash = false;
                }

                if (msg[i] == ' ' && FindSpace == true)
                {
                    endindex = i - 1;
                    if (IsFirstValue)
                    {
                        Head = msg.Substring(startindex, endindex - startindex + 1);
                        IsFirstValue = false;
                    }
                    else
                        Splitmsg.Add(msg.Substring(startindex, endindex - startindex + 1));
                    startindex = i + 1;
                }
            }
            if (IsFirstValue)
            {
                Head = msg.Substring(startindex);
                IsFirstValue = false;
            }
            else
                Splitmsg.Add(msg.Substring(startindex));

            return new ParsedArgsList(Head, Splitmsg);
        }

        private string RemoveSpace(string msg)
        {
            msg = msg.Trim();

            StringBuilder stringBuilder = new StringBuilder();

            bool PrevIsSpace = false;
            bool PrevIsOSlash = false;
            bool FindSpace = true;

            for (int i = 0; i < msg.Length; i++)
            {
                if (msg[i] == '\"' && PrevIsOSlash == false)
                {
                    FindSpace = !FindSpace;
                }

                if (msg[i] == '\\')
                {
                    PrevIsOSlash = true;
                }
                else
                {
                    PrevIsOSlash = false;
                }

                if (FindSpace)
                {
                    if (msg[i] == ' ')
                    {
                        if (!PrevIsSpace)
                        {
                            stringBuilder.Append(msg[i]);
                        }
                        PrevIsSpace = true;
                    }
                    else
                    {
                        stringBuilder.Append(msg[i]);
                        PrevIsSpace = false;
                    }
                }
                else
                {
                    stringBuilder.Append(msg[i]);
                    PrevIsSpace = false;
                }
            }

            return stringBuilder.ToString();
        }

        string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        #endregion
    }
}
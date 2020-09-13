using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using DiscordLinker_2.X.IPCManage;
using DiscordLinker_2.X.OptionManage;
using DiscordLinker_2.X.DiscordManage;
using Discord.WebSocket;

namespace DiscordLinker_2.X
{
    class Program
    {
        private static CountdownEvent CountdownEvent;
        private static ServerManager ServerManager;
        private static ConfigManager ConfigManager;
        private static DiscordServer DiscordServer;

        static void Main(string[] args)
        {
            CountdownEvent = new CountdownEvent(1);
            ConfigManager = new ConfigManager();
            if (ConfigManager.ConfigInit() == false)
                return;

            if (ConfigManager.Config.SetWindowSize)
            {
                Console.SetWindowSize(76, 13);
                Console.SetBufferSize(76, 13);
            }
            Console.WriteLine(
                   @"  ______  _                            _  _      _         _               " + "\n" +
                   @"  |  _  \(_)                          | || |    (_)       | |              " + "\n" +
                   @"  | | | | _  ___   ___  ___   _ __  __| || |     _  _ __  | | __ ___  _ __ " + "\n" +
                   @"  | | | || |/ __| / __|/ _ \ | '__|/ _` || |    | || '_ \ | |/ // _ \| '__|" + "\n" +
                   @"  | |/ / | |\__ \| (__| (_) || |  | (_| || |____| || | | ||   <|  __/| |   " + "\n" +
                   @"  |___/  |_||___/ \___|\___/ |_|   \__,_|\_____/|_||_| |_||_|\_\\___||_|   " + "\n" +
                   "\n" +
                   "  V2.0.2                                                    made by noname" +
                   "\n"
                   );

            ServerManager = new ServerManager(ConfigManager.Config.ServerPort, CountdownEvent);
            ServerManager.Start();
            Console.WriteLine("Http REST Server Started at {0}.", ConfigManager.Config.ServerPort);
            DiscordServer = new DiscordServer(ConfigManager.Config.BotToken);
            DiscordServer.StartAsync();
            Console.WriteLine("Discord Bot Server Started.");

            Thread.Sleep(2000);
            DiscordServer.DiscordSocketClient.MessageReceived += DiscordSocketClient_MessageReceived;
            ServerManager.OnPOSTRequest += ServerManager_OnPOSTRequest;
            Console.WriteLine("DiscordLinker Started.");
            CountdownEvent.Wait();
        }

        private static void ServerManager_OnPOSTRequest(JArray jArray)
        {
            MessageBuilder messageBuilder = new MessageBuilder();

            foreach (var item in jArray)
            {
                PluginRequestObject pluginRequestObject = item.ToObject<PluginRequestObject>();
                messageBuilder.AddMessage(new ChatRoomInfo(pluginRequestObject.Guild, pluginRequestObject.Channel), pluginRequestObject.Msg);
            }

            foreach (var item in messageBuilder.Messages)
            {
                string fullcontent = item.Value.ToString();
                int chunkSize = 2000;
                for (int i = 0; i < fullcontent.Length; i += chunkSize)
                {
                    while (true)
                    {
                        try
                        {
                            (DiscordServer?.DiscordSocketClient?
                                .GetGuild(item.Key.Guild)?
                                .GetChannel(item.Key.Channel) as ISocketMessageChannel)?
                                .SendMessageAsync(fullcontent.Substring(i, (fullcontent.Length < i + chunkSize) ? fullcontent.Length - i : chunkSize))?.Wait();
                        }
                        catch (System.AggregateException)
                        {
                            continue;
                        }
                        break;
                    }
                }
            }
        }
        private static Task DiscordSocketClient_MessageReceived(SocketMessage arg)
        {
            SocketUserMessage socketUserMessage = arg as SocketUserMessage;

            if (socketUserMessage.Author.Id == DiscordServer.DiscordSocketClient.CurrentUser.Id)
                return null;

            LinkerRequestObject linkerRequestObject = new LinkerRequestObject(socketUserMessage);

            JObject jobject = JObject.FromObject(linkerRequestObject);
            ServerManager.JsonEnqueue(jobject);
            return null;
        }
    }
}

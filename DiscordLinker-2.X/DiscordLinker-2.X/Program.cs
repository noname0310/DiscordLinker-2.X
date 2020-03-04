using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DiscordLinker_2.X.IPCManage;
using DiscordLinker_2.X.OptionManage;
using DiscordLinker_2.X.DiscordManage;

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
            Console.WriteLine(
                   @"  ______  _                            _  _      _         _               " + "\n" +
                   @"  |  _  \(_)                          | || |    (_)       | |              " + "\n" +
                   @"  | | | | _  ___   ___  ___   _ __  __| || |     _  _ __  | | __ ___  _ __ " + "\n" +
                   @"  | | | || |/ __| / __|/ _ \ | '__|/ _` || |    | || '_ \ | |/ // _ \| '__|" + "\n" +
                   @"  | |/ / | |\__ \| (__| (_) || |  | (_| || |____| || | | ||   <|  __/| |   " + "\n" +
                   @"  |___/  |_||___/ \___|\___/ |_|   \__,_|\_____/|_||_| |_||_|\_\\___||_|   " + "\n" +
                   "\n" +
                   "  V2.0.0                                                    made by noname" +
                   "\n"
                   );

            CountdownEvent = new CountdownEvent(1);

            ConfigManager = new ConfigManager();
            if (ConfigManager.ConfigInit() == false)
                return;

            ServerManager = new ServerManager(ConfigManager.Config.ServerPort, CountdownEvent);
            ServerManager.Start();
            Console.WriteLine("Http REST Server Started.");
            DiscordServer = new DiscordServer(ConfigManager.Config.BotToken);
            DiscordServer.StartAsync();
            DiscordServer.OnDiscordEventHandled += DiscordServer_OnDiscordEventHandled;
            Console.WriteLine("Discord Bot Server Started.");
            CountdownEvent.Wait();
        }

        private static Task DiscordServer_OnDiscordEventHandled(DiscordEventInfo eventInfo)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                //ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                MaxDepth = 1
            };

            var jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
            Console.WriteLine(JObject.FromObject(eventInfo, jsonSerializer));
            return null;
        }
    }
}

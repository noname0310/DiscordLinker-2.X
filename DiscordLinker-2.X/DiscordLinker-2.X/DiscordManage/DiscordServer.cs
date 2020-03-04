using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;

namespace DiscordLinker_2.X.DiscordManage
{
    class DiscordServer
    {
        public delegate Task DiscordEventHandled(DiscordEventInfo eventInfo);
        public event DiscordEventHandled OnDiscordEventHandled;

        public string APIKey;

        private DiscordSocketClient DiscordSocketClient;

        public DiscordServer(string apiKey)
        {
            APIKey = apiKey;
        }

        public void StartAsync()
        {
            Start().Wait();
        }

        private async Task Start()
        {
            DiscordSocketClient = new DiscordSocketClient();

            await DiscordSocketClient.LoginAsync(TokenType.Bot, APIKey, true);
            await DiscordSocketClient.StartAsync();

            DynamicEventHandler.HandleAllEvents(DiscordSocketClient);
            DynamicEventHandler.OnEventHandled += DynamicEventHandler_OnEventHandled;
        }

        private Task DynamicEventHandler_OnEventHandled(EventInfo eventInfo, params object[] p)
        {
            return OnDiscordEventHandled?.Invoke(new DiscordEventInfo(eventInfo, p));
        }
    }
}

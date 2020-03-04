using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace DiscordLinker_2.X.DiscordManage
{
    class DiscordServer
    {
        public string APIKey;

        public DiscordSocketClient DiscordSocketClient { get; private set; }

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
        }

        public void StopAsync()
        {
            Stop().Wait();
        }

        private async Task Stop()
        {
            await DiscordSocketClient.LogoutAsync();
            await DiscordSocketClient.StopAsync();
            DiscordSocketClient.Dispose();
        }
    }
}

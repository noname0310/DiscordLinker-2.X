using Newtonsoft.Json;

namespace DiscordLinker_2.X.OptionManage
{
    public class DiscordLinkerConfig
    {
        [JsonProperty("Http REST Server Port")]
        public int ServerPort;

        [JsonProperty("Discord Bot Token")]
        public string BotToken;

        public DiscordLinkerConfig()
        {
            //for json serialize
        }
    }
}

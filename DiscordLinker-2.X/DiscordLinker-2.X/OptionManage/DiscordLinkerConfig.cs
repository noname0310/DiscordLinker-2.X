using Newtonsoft.Json;

namespace DiscordLinker_2.X.OptionManage
{
    public class DiscordLinkerConfig
    {
        [JsonProperty("Set Window size")]
        public bool SetWindowSize;

        [JsonProperty("Http REST Server Port")]
        public int ServerPort;

        [JsonProperty("Discord Bot Token")]
        public string BotToken;

        public DiscordLinkerConfig()
        {
            //for json serialize
        }
        public static DiscordLinkerConfig GetDefaultConfig()
        {
            return new DiscordLinkerConfig()
            {
                SetWindowSize = true,
                ServerPort = 20311,
                BotToken = "Please Enter Your Bot Token"
            };
        }
    }
}

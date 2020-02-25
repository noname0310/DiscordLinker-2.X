using Newtonsoft.Json;

namespace DiscordLinker_2.X.Properties.OptionManage
{
    public class DiscordLinkerConfig
    {
        [JsonProperty("server oxide folder path")]
        public string OxideFolderPath;

        public DiscordLinkerConfig()
        {

        }

        public DiscordLinkerConfig(string oxideFolderPath)
        {
            OxideFolderPath = oxideFolderPath;
        }
    }
}

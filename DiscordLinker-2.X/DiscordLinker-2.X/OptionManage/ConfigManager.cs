using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DiscordLinker_2.X.OptionManage
{
    public class ConfigManager
    {
        public DiscordLinkerConfig Config;

        private const string CONFIG_FLIENAME = "DiscordLinkerSettings.json";
        private FileInfo ConfigFileInfo;

        public ConfigManager()
        {
            ConfigFileInfo = new FileInfo(CONFIG_FLIENAME);
        }

        public bool ConfigInit()
        {
            if (!ConfigFileInfo.Exists)
            {
                Config = DiscordLinkerConfig.GetDefaultConfig();
                using(StreamWriter streamWriter = File.CreateText(CONFIG_FLIENAME))
                {
                    streamWriter.Write(JObject.FromObject(Config).ToString());
                    streamWriter.Flush();
                    streamWriter.Close();
                    streamWriter.Dispose();
                }

                Console.WriteLine("{0} File created successfully Please check the contents.", CONFIG_FLIENAME);
                return false;
            }

            JObject jObject = JObject.Parse(File.ReadAllText(CONFIG_FLIENAME));
            Config = jObject.ToObject<DiscordLinkerConfig>();

            if (Config.BotToken == "Please Enter Your Bot Token")
            {
                Console.WriteLine("Entering a discord bot token is mandatory!");
                return false;
            }

            return true;
        }
    }
}

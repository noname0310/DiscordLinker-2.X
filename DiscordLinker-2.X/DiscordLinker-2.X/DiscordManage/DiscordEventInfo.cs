using System.Reflection;

namespace DiscordLinker_2.X.DiscordManage
{
    class DiscordEventInfo
    {
        public string EventName;
        public object[] P;

        public DiscordEventInfo()
        {

        }

        public DiscordEventInfo(EventInfo eventInfo, object[] p)
        {
            EventName = eventInfo.Name;
            P = p;
        }
    }
}

using System.Collections.Generic;
using System.Text;

namespace DiscordLinker_2.X.IPCManage
{
    class MessageBuilder
    {
        public IReadOnlyDictionary<ChatRoomInfo, StringBuilder> Messages { get { return keyValuePairs; } }
        private Dictionary<ChatRoomInfo, StringBuilder> keyValuePairs;

        public MessageBuilder()
        {
            keyValuePairs = new Dictionary<ChatRoomInfo, StringBuilder>();
        }

        public void AddMessage(ChatRoomInfo chatRoomInfo, string str)
        {
            if (!keyValuePairs.ContainsKey(chatRoomInfo))
            {
                StringBuilder stringBuilder = new StringBuilder(str);
                keyValuePairs.Add(chatRoomInfo, stringBuilder);
            }
            else
            {
                keyValuePairs[chatRoomInfo].Append('\n').Append(str);
            }
        }
    }
}

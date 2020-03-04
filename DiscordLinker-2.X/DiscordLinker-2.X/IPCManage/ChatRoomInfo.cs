namespace DiscordLinker_2.X.IPCManage
{
    struct ChatRoomInfo
    {
        public ulong Guild;
        public ulong Channel;

        public ChatRoomInfo(ulong guild, ulong channel)
        {
            Guild = guild;
            Channel = channel;
        }
    }
}

using Discord.WebSocket;

namespace DiscordLinker_2.X.IPCManage
{
    class LinkerRequestObject
    {
        public ulong Guild;
        public ulong ChannelId;
        public ulong AuthorId;
        public string AuthorName;
        public string Msg;

        public LinkerRequestObject()
        {

        }

        public LinkerRequestObject(SocketUserMessage socketUserMessage)
        {
            Guild = ((SocketGuildChannel)socketUserMessage.Channel).Guild.Id;
            ChannelId = socketUserMessage.Channel.Id;
            AuthorId = socketUserMessage.Author.Id;
            AuthorName = socketUserMessage.Author.Username;
            Msg = socketUserMessage.Content;
        }
    }
}

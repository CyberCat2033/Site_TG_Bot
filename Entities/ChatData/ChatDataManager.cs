using System.Collections.Concurrent;

public static class ChatDataManager
{
    public static ConcurrentDictionary<long, ChatData> ChatDatas = new();

    public static void TryAdd(long chatId)
    {
        if (!ChatDatas.TryAdd(chatId, new ChatData()))
        {
            throw new ArgumentException("Bot is already started");
        }
    }

    public static Task<ChatData> TryGetChatData(long chatId)
    {
        if (ChatDatas.TryGetValue(chatId, out var chatData))
        {
            return Task.FromResult(chatData);
        }
        throw new ArgumentException("Please start bot first");
    }
}

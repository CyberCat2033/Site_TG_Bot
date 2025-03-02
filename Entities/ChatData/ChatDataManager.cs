using System.Collections.Concurrent;
using Telegram.Bot;

public static class ChatDataManager
{
    private static ConcurrentDictionary<long, ChatData> ChatDataDictionary = new();

    public static bool TryAdd(long chatId, TelegramBotClient bot)
    {
        if (ChatDataDictionary.TryAdd(chatId, new ChatData(bot, chatId)))
        {
            return true;
        }
        return false;
    }

    public static ChatData GetChatData(long chatId)
    {
        if (ChatDataDictionary.TryGetValue(chatId, out var chatData))
        {
            return chatData;
        }
        throw new ArgumentException("Please start bot first");
    }
}

using Telegram.Bot;
using Telegram.Bot.Types;

public static class NewReport
{
    public static async Task ExecuteAsync(
        Message message,
        TelegramBotClient client,
        CancellationToken canTok
    )
    {
        var data = ChatDataManager.GetChatData(message.Chat.Id);
        if (data.userSession.IsNull())
        {
            data.AddReport(await data.userSession.CreateReport());
        }
    }
}

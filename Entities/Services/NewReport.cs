using Telegram.Bot;
using Telegram.Bot.Types;

public static class NewReport
{
    public static async Task ExecuteAsync(
        Message message,
        TelegramBotClient client,
        CancellationToken canTok,
        UserRepository userRepository
    )
    {
        var user = await userRepository.GetByChatIdAsync(message.Chat.Id);
        if (user == null)
        {
            throw new ArgumentException("Сначала запустите бота");
        }
        var handler = Bot.handlers[message.Chat.Id];
        if (handler.IsComplete())
        {
            handler.CreateReport();
        }
        // var data = ChatDataManager.GetChatData(message.Chat.Id);
        // if (data.userSession.IsNull())
        // {
        //     data.AddReport(await data.userSession.CreateReport());
        // }
    }
}

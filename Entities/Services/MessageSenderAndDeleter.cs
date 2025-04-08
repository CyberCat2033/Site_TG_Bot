using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

public static class MessageSenderAndDeleter
{
    public static async Task SendMessageAndDeleteAsync(
        Message message,
        TelegramBotClient client,
        CancellationToken cancelationToken,
        string text,
        ReplyKeyboardMarkup? replyKeyboard = null
    )
    {
        await client.SendMessage(
            chatId: message.Chat.Id,
            text: text,
            replyMarkup: replyKeyboard,
            cancellationToken: cancelationToken
        );
        await client.DeleteMessage(message.Chat.Id, message.MessageId, cancelationToken);
        await Task.Delay(1000);
    }
}

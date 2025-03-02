using Telegram.Bot;
using Telegram.Bot.Types;

public static class MessageSenderAndDeleter
{
    public static async Task SendMessageAndDeleteAsync(
        Message message,
        TelegramBotClient client,
        CancellationToken cancelationToken,
        string text,
        bool DeleteSelfMessage = true
    )
    {
        await client.SendMessage(
            chatId: message.Chat.Id,
            text: text,
            cancellationToken: cancelationToken
        );
        await client.DeleteMessage(message.Chat.Id, message.MessageId, cancelationToken);
        await Task.Delay(1000);
        if (DeleteSelfMessage)
            await client.DeleteMessage(message.Chat.Id, message.MessageId + 1, cancelationToken);
    }
}

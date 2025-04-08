using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

public static class StartCommand
{
    private static ReplyKeyboardMarkup replyKeyboard = new(
        new[]
        {
            new KeyboardButton("📝 Новая анкета о пропаже"),
            new KeyboardButton("📋 Мои анкеты"),
        }
    )
    {
        ResizeKeyboard = true,
    };

    public static async Task ExecuteAsync(
        Message message,
        TelegramBotClient client,
        CancellationToken canTok,
        UserRepository repository
    )
    {
        string GreatingText;

        if (await repository.Contains(message.Chat.Id))
        {
            GreatingText = "Бот и так запущен";
        }
        else
        {
            await repository.Add(Guid.NewGuid(), message);
            GreatingText =
                $"Привет мое имя {Bot.Name}. \nЯ помогу тебе создать заявление о пропаже.\nПросто нажми на кнопку \"📝 Новая анкета о пропаже\"";
        }

        await MessageSenderAndDeleter.SendMessageAndDeleteAsync(
            message,
            client,
            canTok,
            GreatingText,
            replyKeyboard: replyKeyboard
        );
    }
}

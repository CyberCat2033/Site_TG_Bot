using Telegram.Bot;
using Telegram.Bot.Types;

public class StartCommand : TelegramBotCommand
{
    public StartCommand(string command, string description)
        : base(command, description) { }

    public override async Task ExecuteAsync(
        Message message,
        TelegramBotClient client,
        CancellationToken canTok
    )
    {
        string GreatingText;
        if (ChatDataManager.TryAdd(message.Chat.Id, client))
        {
            GreatingText = $"Hi, my name is {Bot.Name}.";
        }
        else
            GreatingText = "The bot is already running";
        await MessageSenderAndDeleter.SendMessageAndDeleteAsync(
            message,
            client,
            canTok,
            GreatingText
        );
    }
}

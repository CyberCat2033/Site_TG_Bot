using Telegram.Bot;
using Telegram.Bot.Types;

public abstract class TelegramBotCommand : BotCommand
{
    public TelegramBotCommand(string command, string description)
    {
        base.Description = description;
        base.Command = command;
    }

    public abstract Task ExecuteAsync(
        Message message,
        TelegramBotClient client,
        CancellationToken canTok
    );
}

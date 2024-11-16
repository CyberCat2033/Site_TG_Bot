using Telegram.Bot;
using Telegram.Bot.Types;

public class StartCommand : IBotCommand
{
    public StartCommand() { }

    public async Task ExecuteAsync(
        Message message,
        ITelegramBotClient client,
        CancellationToken canTok
    )
    {
        throw new NotImplementedException();
    }
}

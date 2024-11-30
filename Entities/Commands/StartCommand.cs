using Telegram.Bot;
using Telegram.Bot.Types;

public class StartCommand : IBotCommand
{
    public StartCommand(string Message) { }

    public async Task ExecuteAsync(
        Message message,
        ITelegramBotClient client,
        CancellationToken canTok
    )
    {
        throw new NotImplementedException();
    }
}

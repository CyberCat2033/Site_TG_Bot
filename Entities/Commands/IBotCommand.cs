using Telegram.Bot;
using Telegram.Bot.Types;

public interface IBotCommand
{
    public Task ExecuteAsync(Message message, ITelegramBotClient client, CancellationToken canTok);
}

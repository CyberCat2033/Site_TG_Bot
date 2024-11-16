// using Microsoft.Extensions.Diagnostics.HealthChecks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

// using Telegram.Bot.Types.Enums;


public class Bot
{
    private ITelegramBotClient botClient { get; init; }
    public string? StartTime { get; private set; }
    public string? EndTime { get; private set; }
    private ReceiverOptions receiverOptions { get; init; }
    private readonly Dictionary<string, IBotCommand> commandDict;
    public string? Name { get; private set; }
    private static Bot? instance;
    private CancellationToken canTok { get; init; }

    private Bot(ITelegramBotClient _botClient, CancellationToken _canTok, string _name)
    {
        Name = _name;
        botClient = _botClient;
        canTok = _canTok;
        receiverOptions = new() { AllowedUpdates = { }, DropPendingUpdates = true };
        commandDict = new() { };
    }

    public static async Task<Bot> GetInstanceAsync(string token, CancellationToken cancellation)
    {
        if (instance == null)
        {
            var _botClient = new TelegramBotClient(token);
            var _name = (await _botClient.GetMyNameAsync());
            instance = new Bot(_botClient, cancellation, _name.Name);
        }
        return instance;
    }

    private async Task startNotification()
    {
        StartTime = DateTime.Now.ToString();
        Console.ForegroundColor = ConsoleColor.Green;
        await Console.Out.WriteLineAsync($"The {Name} Bot has been started at {StartTime}");
    }

    public async Task Start()
    {
        await startNotification();
        await botClient.ReceiveAsync(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            receiverOptions,
            canTok
        );
    }

    private async Task HandlePollingErrorAsync(
        ITelegramBotClient client,
        Exception exception,
        CancellationToken token
    )
    {
        await Console.Out.WriteLineAsync(exception.Message);
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient client,
        Update update,
        CancellationToken token
    )
    {
        // await client.SendTextMessageAsync(update.Message.Chat.Id, "Hi");
    }
}

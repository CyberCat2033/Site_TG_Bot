using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class Bot
{
    private UserSession session;
    private TelegramBotClient botClient { get; init; }
    public string? StartTime { get; private set; }
    public string? StopTime { get; private set; }
    private ReceiverOptions receiverOptions { get; init; }
    private readonly Dictionary<string, TelegramBotCommand> commandDict;
    public static string? Name { get; private set; }
    private static Bot? instance;
    private CancellationTokenSource cts { get; init; }

    private const string StartMessage = "Бот был запущенн";

    private Bot(TelegramBotClient _botClient, CancellationTokenSource _cts, string _name)
    {
        Name = _name;
        botClient = _botClient;
        cts = _cts;
        receiverOptions = new() { AllowedUpdates = { }, DropPendingUpdates = true };
        commandDict = new()
        {
            ["/start"] = new StartCommand("/start", "Start the bot"),
            ["/newreport"] = new NewReportCommand("/newreport", "Create a new report"),
        };
        _botClient.SetMyCommands(commandDict.Select(x => x.Value));
    }

    public static async Task<Bot> GetInstanceAsync(string token, CancellationTokenSource _cts)
    {
        if (instance == null)
        {
            var _botClient = new TelegramBotClient(token);
            var _name = (await _botClient.GetMyName());
            instance = new Bot(_botClient, _cts, _name.Name);
        }
        return instance;
    }

    private async Task startNotification()
    {
        StartTime = DateTime.Now.ToString();
        Console.ForegroundColor = ConsoleColor.Green;
        await Console.Out.WriteLineAsync($"The {Name} Bot has been started at {StartTime}");
        Console.ResetColor();
    }

    public async Task Start()
    {
        await startNotification();
        await botClient.ReceiveAsync(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            receiverOptions,
            cts.Token
        );
    }

    public async Task Stop()
    {
        StopTime = DateTime.Now.ToString();
        cts.Cancel();
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.ForegroundColor = ConsoleColor.Yellow;
        await Console.Out.WriteAsync($"The Bot was stopped at {StopTime}");
        Console.ResetColor();
    }

    private static async Task LogError(Exception e)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        await Console.Out.WriteLineAsync($"Exception:{e}, exception message: {e.Message}");
        Console.ResetColor();
    }

    private async Task HandlePollingErrorAsync(
        ITelegramBotClient client,
        Exception exception,
        CancellationToken token
    )
    {
        await LogError(exception);
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient client,
        Update update,
        CancellationToken token
    )
    {
        var message = update.Message;

        if (message is not { })
            return;
        if (message.Text is not { } messageText)
            return;

        if (message is { Type: MessageType.Text } && messageText.StartsWith("/"))
        {
            if (commandDict.TryGetValue(messageText.ToLower().Split()[0], out var telegramCommand))
            {
                try
                {
                    await telegramCommand.ExecuteAsync(message, botClient, token);
                    return;
                }
                catch (Exception exception)
                {
                    await client.SendMessage(
                        chatId: message.Chat.Id,
                        text: exception.Message,
                        cancellationToken: token
                    );
                    await LogError(exception);
                    return;
                }
            }
        }
        try
        {
            var data = ChatDataManager.GetChatData(message.Chat.Id);
            session = data.userSession;
            if (!session.IsNull())
            {
                await session.HandleInput(messageText);
                await client.SendMessage(message.Chat.Id, "OK");
            }
        }
        catch (Exception ex)
        {
            await client.SendMessage(message.Chat.Id, ex.Message, cancellationToken: token);
        }
    }
}

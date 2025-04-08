using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class Bot
{
    public readonly TelegramBotClient botClient;
    private readonly ReceiverOptions receiverOptions;
    private readonly CancellationTokenSource cts;

    public static Dictionary<long, LostReportFlowHandler> handlers = new();
    public static Bot? Instance { get; private set; }
    public static string? Name { get; private set; }
    public string? StartTime { get; private set; }
    public string? StopTime { get; private set; }
    public static UserRepository? userRepository;
    public static LostReportRepository? lostReportRepository;
    public static BotContext? context;

    private Bot(TelegramBotClient botClient, CancellationTokenSource cts, string name)
    {
        Name = name;
        this.botClient = botClient;
        this.cts = cts;

        receiverOptions = new ReceiverOptions { AllowedUpdates = { }, DropPendingUpdates = true };
        var optionsBuilder = new DbContextOptionsBuilder<BotContext>();
        var options = optionsBuilder
            .UseSqlite("Data Source=/home/cybercat/Site_TG_Bot/DataBases/bot.db")
            .LogTo(message => System.Diagnostics.Debug.WriteLine(message))
            .Options;
        context = new BotContext(options);
        userRepository = new(context);
        lostReportRepository = new(context);
    }

    public static async Task<Bot> GetInstanceAsync(string token, CancellationTokenSource cts)
    {
        if (Instance == null)
        {
            var botClient = new TelegramBotClient(token);
            var botName = (await botClient.GetMyName()).Name;
            Instance = new Bot(botClient, cts, botName);
        }
        return Instance;
    }

    public async Task StartAsync()
    {
        StartTime = DateTime.Now.ToString();
        await LogMessage($"The {Name} Bot has been started at {StartTime}", ConsoleColor.Green);

        await botClient.ReceiveAsync(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            receiverOptions,
            cts.Token
        );
    }

    public async Task StopAsync()
    {
        StopTime = DateTime.Now.ToString();
        await cts.CancelAsync();
        await LogMessage(
            $"The Bot was stopped at {StopTime}",
            ConsoleColor.Yellow,
            ConsoleColor.DarkRed
        );
    }

    private static async Task LogMessage(
        string message,
        ConsoleColor foreground,
        ConsoleColor? background = null
    )
    {
        if (background.HasValue)
            Console.BackgroundColor = background.Value;

        Console.ForegroundColor = foreground;
        await Console.Out.WriteLineAsync($"{DateTime.Now:HH:mm:ss} - {message}");
        Console.ResetColor();
    }

    private static async Task LogError(Exception e)
    {
        await LogMessage($"Exception: {e}\nMessage: {e.Message}", ConsoleColor.Red);
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
        await LogMessage($"Update {update.Id} received\nType: {update.Type}", ConsoleColor.Green);

        if (update.CallbackQuery is { } callbackquery)
        {
            var queryId = callbackquery.From.Id;
            if (!handlers.ContainsKey(queryId))
            {
                handlers.TryAdd(
                    queryId,
                    new LostReportFlowHandler(lostReportRepository, Instance.botClient)
                );
            }
            var user = await userRepository.GetByChatIdAsync(chatId: callbackquery.From.Id);
            var handlerFlow = handlers[queryId];
            await client.AnswerCallbackQuery(callbackquery.Id);
            if (!handlerFlow.IsComplete())
                return;
            switch (callbackquery.Data.Split()[0])
            {
                case "selectReport":
                    await SelectReport.ExecuteAsync(
                        callbackquery.Data.Split()[1],
                        callbackquery.From.Id,
                        user,
                        botClient,
                        token,
                        lostReportRepository
                    );
                    return;

                case "changeDescription":
                    handlerFlow.EditDescription();
                    break;
                case "changeFeedback":
                    handlerFlow.EditFeedback();
                    break;
                case "changeLocation":
                    handlerFlow.EditLocation();
                    break;
                case "changeName":
                    handlerFlow.EditName();
                    break;
                case "changePhoto":
                    handlerFlow.EditPhoto();
                    break;
                default:
                    return;
            }
            await botClient.SendMessage(queryId, handlerFlow.GetCurrentPrompt());
            return;
        }
        var message = update.Message;
        // if (message.Type != MessageType.Text && message.Type != MessageType.Photo)
        // {
        //     return;
        // }
        var messageText = message.Text ?? "";
        var chatId = message.Chat.Id;
        if (!handlers.ContainsKey(chatId))
        {
            handlers.TryAdd(
                chatId,
                new LostReportFlowHandler(lostReportRepository, Instance.botClient)
            );
        }

        try
        {
            if (messageText == "/start")
            {
                await StartCommand.ExecuteAsync(message, botClient, token, userRepository);
                return;
            }
            else
            {
                switch (messageText)
                {
                    case "📝 Новая анкета о пропаже":
                        await NewReport.ExecuteAsync(message, botClient, token, userRepository);
                        var handlerFow = handlers[chatId];
                        await botClient.SendMessage(chatId, handlerFow.GetCurrentPrompt());
                        return;
                    case "📋 Мои анкеты":
                        await ListReports.ExecuteAsync(
                            message,
                            botClient,
                            token,
                            lostReportRepository
                        );
                        return;
                }
            }

            // var user = await userRepository.GetByChatIdAsync(chatId: message.Chat.Id);
            var handlerFlow = handlers[chatId];
            if (!handlerFlow.IsComplete())
            {
                await handlerFlow.ProcessInput(message);
                if (handlerFlow.IsComplete())
                {
                    await botClient.SendMessage(chatId, "Заполнение успешно законченно");
                    return;
                }

                await botClient.SendMessage(chatId, handlerFlow.GetCurrentPrompt());
            }
        }
        catch (ArgumentException ex)
        {
            await client.SendMessage(message.Chat.Id, ex.Message, cancellationToken: token);
            await botClient.SendMessage(chatId, handlers[chatId].GetCurrentPrompt());
        }
    }
}

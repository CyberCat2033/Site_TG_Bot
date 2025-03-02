using Telegram.Bot;

public class UserSession
{
    private static TelegramBotClient? _bot;
    private ILostReport? _report;
    private readonly Queue<Func<ILostReport, string, Task<bool>>> _steps = new();
    private bool IsNotify = true;
    private long _chatId;

    public UserSession(TelegramBotClient bot, long chatId)
    {
        _bot = bot;
        _chatId = chatId;
    }

    public bool IsNull() => _steps.Count == 0;

    private Task<bool> SetAsyncNotify(
        Func<string, Task<bool>> function,
        string messageOutput,
        string messageInput
    )
    {
        if (IsNotify)
        {
            IsNotify = false;
            return _bot.SendMessage(_chatId, messageOutput).ContinueWith(_ => false);
        }
        IsNotify = true;
        return function(messageInput);
    }

    private Task<bool> SetNameAsyncNotify(ILostReport report, string messageInput) =>
        SetAsyncNotify(report.SetNameAsync, "Enter Name", messageInput);

    private Task<bool> SetDescriptionAsyncNotify(ILostReport report, string messageInput) =>
        SetAsyncNotify(report.SetDescriptionAsync, "Enter Description", messageInput);

    private Task<bool> SetFeedbackAsyncNotify(ILostReport report, string messageInput) =>
        SetAsyncNotify(report.SetFeedbackAsync, "Enter Feedback", messageInput);

    private Task<bool> SetLocationAsyncNotify(ILostReport report, string messageInput) =>
        SetAsyncNotify(report.SetLocationAsync, "Enter Location", messageInput);

    public void AddStep(Func<ILostReport, string, Task<bool>> step) => _steps.Enqueue(step);

    public ILostReport CreateReport()
    {
        _report = new LostReport();
        AddStep(SetNameAsyncNotify);
        AddStep(SetDescriptionAsyncNotify);
        AddStep(SetFeedbackAsyncNotify);
        AddStep(SetLocationAsyncNotify);
        return _report;
    }

    public async Task HandleInput(string input) => await HandleInput(_report, input);

    public async Task HandleInput(ILostReport report, string input)
    {
        if (_steps.TryPeek(out var step))
        {
            if (await step(report, input))
            {
                _steps.Dequeue();
            }
        }
    }
}

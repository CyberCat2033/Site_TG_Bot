using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class LostReportFlowHandler
{
    private class Step
    {
        public System.Reflection.PropertyInfo? Property { get; init; }
        public string? Prompt { get; init; }
    }

    private LostReportRepository _reportRepository;
    public static readonly string ImagePath = "/home/cybercat/Images/";
    private readonly Queue<Step> _steps = new();
    private LostReport? report;
    private static ITelegramBotClient? _bot;

    public LostReportFlowHandler(LostReportRepository repos, ITelegramBotClient botClient)
    {
        _reportRepository = repos;
        _bot = botClient;
    }

    public bool IsComplete() => _steps.Count() == 0;

    public string? GetCurrentPrompt() =>
        _steps.Peek().Prompt ?? throw new Exception("No more steps");

    public void EditName() => EditProp("Name");

    public void EditDescription() => EditProp("Description");

    public void EditLocation() => EditProp("Location");

    public void EditContact() => EditProp("Contact");

    public void EditFeedback() => EditProp("Feedback");

    public void EditPhoto() => EditProp("PhotoPath");

    public void SelectReport(LostReport rep) => report = rep;

    public void CreateReport()
    {
        SelectReport(new LostReport { Id = Guid.NewGuid() });
        foreach (var prop in typeof(LostReport).GetProperties())
        {
            var attr = prop.GetCustomAttribute<PromptAttribute>();
            if (attr == null)
                continue;

            _steps.Enqueue(new Step { Property = prop, Prompt = attr.Message });
        }
    }

    public async Task ProcessInput(Message input)
    {
        if (IsComplete())
        {
            await _reportRepository.AddOrUpdate(input.Chat.Id, report);
            return;
        }

        var step = _steps.Peek();
        var prop = step.Property!;

        bool isPhotoPath = prop.Name == "PhotoPath";

        if (
            isPhotoPath && input.Type != MessageType.Photo
            || !isPhotoPath && input.Type != MessageType.Text
        )
        {
            throw new ArgumentException("Неверный ввод, попробуйте снова");
        }

        if (isPhotoPath)
        {
            string path = await HandlePhotoMessage(input);
            prop.SetValue(report, path);
        }
        else
        {
            if (!await Checker.GlobalChecker(input.Text))
                throw new ArgumentException("Неверный ввод, попробуйте снова");

            prop.SetValue(report, input.Text);
        }

        _steps.Dequeue();
        if (IsComplete())
        {
            await _reportRepository.AddOrUpdate(input.Chat.Id, report);
            return;
        }
    }

    private void EditProp(string propName)
    {
        var prop = typeof(LostReport).GetProperty(
            propName,
            BindingFlags.Public | BindingFlags.Instance
        );
        if (prop == null)
            throw new Exception($"Свойство '{propName}' не найдено в LostReport2");

        var attr = prop.GetCustomAttribute<PromptAttribute>();
        _steps.Enqueue(new Step { Property = prop, Prompt = attr?.Message });
    }

    private async Task<string> HandlePhotoMessage(Message message)
    {
        if (message.Photo == null || message.Photo.Length == 0)
        {
            throw new ArgumentException("Неверный ввод, попробуйте снова");
        }

        var fileId = message.Photo[^1].FileId; // Берём самое большое фото
        var file = await _bot.GetFile(fileId);
        var filePath = Path.Combine(ImagePath, $"{Guid.NewGuid()}.jpg");

        await using var saveStream = new FileStream(filePath, FileMode.Create);
        await _bot.DownloadFile(file.FilePath, saveStream);
        return filePath;
    }
}

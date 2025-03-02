using Telegram.Bot;

public class ChatData
{
    public readonly ILocale Locale = LocalizationManager.GetLocale("en");

    public List<ILostReport> reports = new();

    public UserSession userSession;

    public ChatData(TelegramBotClient bot, long chatId)
    {
        userSession = new UserSession(bot, chatId);
    }

    public void AddReport(ILostReport report)
    {
        reports.Add(report);
    }

    public ChatData() { }
}

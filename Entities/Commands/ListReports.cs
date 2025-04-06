using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public static class ListReports
{
    public static async Task ExecuteAsync(
        Message message,
        TelegramBotClient client,
        CancellationToken canTok
    )
    {
        var data = ChatDataManager.GetChatData(message.Chat.Id);
        var reports = data.reports;
        if (!data.userSession.IsNull())
        {
            return;
        }
        if (reports.Count == 0)
        {
            await client.SendMessage(
                message.Chat.Id,
                "У вас отсутствуют заявления о пропаже",
                cancellationToken: canTok
            );
            return;
        }
        ushort reportIndex = 0;
        foreach (var report in reports)
        {
            await SendReport(client, message.Chat.Id, report, canTok, reportIndex);
            ++reportIndex;
        }
    }

    private static async Task SendReport(
        TelegramBotClient client,
        long chatId,
        ILostReport report,
        CancellationToken canTok,
        ushort reportIndex
    )
    {
        string photoPath = report.PhotoPath;
        await using var stream = new FileStream(photoPath, FileMode.Open, FileAccess.Read);

        var keyboard = new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData("📝 Изменить поля", $"selectReport {reportIndex}")
        );

        await client.SendPhoto(
            chatId: chatId,
            photo: InputFile.FromStream(stream),
            caption: report.Show(),
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: canTok
        );
    }
}

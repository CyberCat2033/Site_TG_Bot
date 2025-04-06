using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public static class SelectReportInline
{
    public static async Task ExecuteAsync(
        string input,
        ChatData data,
        long chatId,
        TelegramBotClient client,
        CancellationToken canTok
    )
    {
        int report_index = int.Parse(input.Split()[1]);
        var report = data.GetReport(report_index);
        await SendReportReplyKeyboard(client, chatId, report, canTok);
        data.userSession.SetReport(report);
    }

    private static async Task SendReportReplyKeyboard(
        TelegramBotClient client,
        long chatId,
        ILostReport report,
        CancellationToken canTok
    )
    {
        var inlineKeyboard = new InlineKeyboardMarkup(
            new InlineKeyboardButton[][]
            {
                new[] { InlineKeyboardButton.WithCallbackData("✏ Изменить имя", "changeName") },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        "📝 Изменить описание",
                        "changeDescription"
                    ),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        "📞 Изменить способ связи",
                        "changeFeedback"
                    ),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        "📍 Изменить место поиска",
                        "changeLocation"
                    ),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📷 Изменить фотографию", "changePhoto"),
                },
            }
        );
        string photoPath = report.PhotoPath;
        await using var stream = new FileStream(photoPath, FileMode.Open, FileAccess.Read);

        await client.SendPhoto(
            chatId,
            photo: InputFile.FromStream(stream),
            caption: report.Show(),
            replyMarkup: inlineKeyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: canTok
        );
    }
}

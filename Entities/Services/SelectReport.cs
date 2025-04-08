using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public static class SelectReport
{
    public static async Task ExecuteAsync(
        string input,
        long chatId,
        User user,
        TelegramBotClient client,
        CancellationToken canTok,
        LostReportRepository repository
    )
    {
        Guid report_index = Guid.Parse(input);
        var report = await repository.GetForUpdate(chatId, report_index);
        await SendReportReplyKeyboard(client, chatId, report, canTok);
        Bot.handlers[chatId].SelectReport(report);
    }

    private static async Task SendReportReplyKeyboard(
        TelegramBotClient client,
        long chatId,
        LostReport report,
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

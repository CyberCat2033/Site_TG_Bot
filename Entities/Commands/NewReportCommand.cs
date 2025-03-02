using Telegram.Bot;
using Telegram.Bot.Types;

class NewReportCommand : TelegramBotCommand
{
    public NewReportCommand(string command, string description)
        : base(command, description) { }

    public override async Task ExecuteAsync(
        Message message,
        TelegramBotClient client,
        CancellationToken canTok
    )
    {
        var data = ChatDataManager.GetChatData(message.Chat.Id);
        data.AddReport(data.userSession.CreateReport());
        data.userSession.HandleInput("");

        //         await MessageSenderAndDeleter.SendMessageAndDeleteAsync(
        //             message,
        //             client,
        //             canTok,
        //             "Enter the name of the lost item"
        //         );
    }
}

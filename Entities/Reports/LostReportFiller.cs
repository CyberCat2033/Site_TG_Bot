using Telegram.Bot;

public class LostReportFiller
{
    private static async Task<bool> FillField(
        ITelegramBotClient client,
        long ID,
        Func<string, bool> action,
        string OKMessage
    )
    {
        await client.SendMessage(ID, "Please enter name");
        if (action.Invoke("Name"))
        {
            return true;
        }
        else
        {
            throw new Exception("Wrong Input.\n Please try again");
        }
    }
}

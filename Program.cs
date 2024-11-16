public class MainClass
{
    public static async Task Main(string[] args)
    {
        string token =
            System.Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
            ?? throw new Exception("No Envirenment Variable avalible");
        CancellationToken cts = new();
        var bot = await Bot.GetInstanceAsync(token, cts);
        await bot.Start();
    }
}

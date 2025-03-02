public class MainClass
{
    public static async Task Main(string[] args)
    {
        // LocalizationManager.LocalizationManagera();
        string token =
            System.Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
            ?? throw new Exception("No Envirenment Variable avalible");
        CancellationTokenSource cts = new();
        var bot = await Bot.GetInstanceAsync(token, cts);
        await bot.Start();
        while (true)
        {
            if (Console.ReadLine()?.ToLower() is not "/stop" and not "/exit")
            {
                continue;
            }
            await bot.Stop();
            break;
        }
    }
}

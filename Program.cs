public class MainClass
{
    public static async Task Main(string[] args)
    {
        string token =
            System.Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
            ?? throw new Exception("No Envirenment Variable avalible");
        CancellationTokenSource cts = new();
        var bot = await Bot.GetInstanceAsync(token, cts);
        await bot.StartAsync();

        // while (true)
        // {
        //     if (Console.ReadLine()?.ToLower() is "/stop" or "/exit")
        //     {
        //         Console.WriteLine("FUCK");
        //         await bot.StopAsync();
        //         break;
        //     }
        // }
        // Console.Read();
    }
}

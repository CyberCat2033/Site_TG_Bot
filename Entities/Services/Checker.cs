public class Checker
{
    public static async Task<bool> GlobalChecker(string? str)
    {
        await Task.Delay(100);
        return str.Length < 900;
    }

    public static async Task<bool> GlobalChecker(long str)
    {
        await Task.Delay(100);
        return true;
    }
}

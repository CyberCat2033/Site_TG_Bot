public class Checker
{
    public static async Task<bool> GlobalChecker(string str)
    {
        await Task.Delay(100);
        return true;
    }
}

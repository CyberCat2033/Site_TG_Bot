public class ChatData
{
    public readonly ILocale locale;

    public List<ILostReport> reports = new();

    public ChatData() { }

    public async Task CreateReportAsync() { }
}

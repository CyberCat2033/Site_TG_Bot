public class LostReport : ILostReport
{
    public string Name { get; private set; } = "";
    public string Description { get; private set; } = "";
    public string Feedback { get; private set; } = "";
    public string Location { get; private set; } = "";

    public LostReport() { }

    public LostReport(string name, string description, string feedback, string location)
    {
        Name = name;
        Description = description;
        Feedback = feedback;
        Location = location;
    }

    private static async Task<bool> SetFieldAsync(Action<string> setter, string value)
    {
        if (await Checker.GlobalChecker(value))
        {
            setter(value);
            return true;
        }
        return false;
    }

    public async Task<bool> SetNameAsync(string input) =>
        await SetFieldAsync(val => Name = val, input);

    public async Task<bool> SetDescriptionAsync(string input) =>
        await SetFieldAsync(val => Description = val, input);

    public async Task<bool> SetFeedbackAsync(string input) =>
        await SetFieldAsync(val => Feedback = val, input);

    public async Task<bool> SetLocationAsync(string input) =>
        await SetFieldAsync(val => Location = val, input);
}

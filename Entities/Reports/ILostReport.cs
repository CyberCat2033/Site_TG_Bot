public interface ILostReport
{
    string Name { get; }
    string Feedback { get; }
    string Description { get; }
    string Location { get; }

    public Task<bool> SetNameAsync(string input);
    public Task<bool> SetDescriptionAsync(string input);
    public Task<bool> SetFeedbackAsync(string input);
    public Task<bool> SetLocationAsync(string input);
}

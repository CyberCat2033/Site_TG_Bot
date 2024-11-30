public class Locale : ILocale
{
    public Locale(string language, Dictionary<string, string> values)
    {
        Language = language;
        this.Values = Values;
    }

    public Dictionary<string, string> Values { get; }

    public string Language { get; private set; }

    public string Translate(string value)
    {
        if (!Values.TryGetValue(value, out var output))
        {
            throw new Exception();
        }
        return output;
    }
}

public interface ILocale
{
    public Dictionary<string, string> Values { get; }

    string Language { get; }

    public string Translate(string value);
}

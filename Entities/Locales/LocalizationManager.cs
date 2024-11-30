using Newtonsoft.Json;

public static class LocalizationManager
{
    public static readonly Dictionary<string, Locale> Locales = new();
    const string localesPath = "/home/cybercat/Site_TG_Bot/DataBases/locales.json";

    static LocalizationManager()
    {
        string json = File.ReadAllText(localesPath);
        var rawData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(
            json
        );

        foreach (var locale in rawData)
        {
            Locales[locale.Key] = new Locale(locale.Key, locale.Value);
        }
    }

    public static Task<Locale> GetLocaleAsync(string language)
    {
        if (Locales.TryGetValue(language, out var locale))
        {
            return Task.FromResult(locale);
        }
        throw new ArgumentException("No Such language was provided");
    }
}

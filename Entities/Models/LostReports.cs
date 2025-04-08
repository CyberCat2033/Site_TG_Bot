public class PromptAttribute : Attribute
{
    public string Message { get; }

    public PromptAttribute(string message) => Message = message;
}

public class LostReport
{
    public Guid Id { get; set; }

    //
    public Guid UserId { get; set; } // внешний ключ

    public User User { get; set; } = null!;

    [Prompt("📌Введите Имя:")]
    public string Name { get; set; } = string.Empty;

    [Prompt("📝Введите Описание")]
    public string Description { get; set; } = string.Empty;

    [Prompt("✍Введите способ связи")]
    public string Feedback { get; set; } = string.Empty;

    [Prompt("📍Введите место поиска")]
    public string Location { get; set; } = string.Empty;

    [Prompt("📷Отправьте фотографию")]
    public string PhotoPath { get; set; } = string.Empty;

    public string Show()
    {
        return $"\U0001F50D *Анкета о пропаже*\n\n"
            + $"📌 *Имя:* {Name}\n\n"
            + $"📝 *Описание:* {Description}\n\n"
            + (!string.IsNullOrEmpty(Feedback) ? $"✍ *Способ связи:* {Feedback}\n\n" : "")
            + (!string.IsNullOrEmpty(Location) ? $"📍 *Местоположение:* {Location}\n" : "");
    }

    public override string ToString() => Show();
}

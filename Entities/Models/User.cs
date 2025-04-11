public class User
{
    public Guid Id { get; set; }

    public long? chatId { get; set; }

    public string? UserName { get; set; }

    public List<LostReport> Reports { get; set; } = [];
}

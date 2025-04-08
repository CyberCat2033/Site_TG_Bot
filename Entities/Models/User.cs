using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public Guid Id { get; set; }

    public long? chatId { get; set; }

    public string UserName { get; set; }

    public List<LostReport> Reports { get; set; } = [];

    [NotMapped]
    public LostReportFlowHandler handler;

    public User()
    {
        handler = new(Bot.lostReportRepository, Bot.Instance.botClient);
    }
}

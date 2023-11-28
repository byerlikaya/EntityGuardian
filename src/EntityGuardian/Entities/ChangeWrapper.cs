
namespace EntityGuardian.Entities;

public class ChangeWrapper
{
    [Key]
    public Guid Guid { get; set; }

    public Guid DbContextId { get; set; }

    public string Username { get; set; }

    public string IpAddress { get; set; }

    public int TransactionCount { get; set; }

    public DateTime TransactionDate { get; set; }

    [NotMapped]
    public string Entities { get; set; }

    public List<Change> Changes { get; set; }
}
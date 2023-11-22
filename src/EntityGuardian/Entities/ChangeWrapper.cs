namespace EntityGuardian.Entities;

public class ChangeWrapper
{
    [Key]
    public Guid Guid { get; set; }

    public string Username { get; set; }

    public string IpAddress { get; set; }

    public string MainEntity { get; set; }

    public int TransactionCount { get; set; }

    public DateTime TransactionDate { get; set; }

    public List<Change> Changes { get; set; }
}
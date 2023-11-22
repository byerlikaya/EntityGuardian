namespace EntityGuardian.Entities;

public class Change
{
    [Key]
    public Guid Guid { get; set; }

    public Guid ChangeWrapperGuid { get; set; }

    public int Rank { get; set; }

    public string TransactionType { get; set; }

    public string EntityName { get; set; }

    public string OldData { get; set; }

    public string NewData { get; set; }

    public DateTime TransactionDate { get; set; }
}
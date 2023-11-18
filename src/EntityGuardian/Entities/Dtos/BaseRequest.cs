namespace EntityGuardian.Entities.Dtos;

public class BaseRequest : IWhereClause
{
    public int Start { get; set; }
    public int Max { get; set; }
    public Sorting OrderBy { get; set; }
}
namespace EntityGuardian.Entities.Dtos;

public class ChangeWrapperRequest : BaseRequest
{
    [TextualWhereClause(StringMethod.Contains)]
    public string MainEntity { get; set; }

    [WhereClause]
    public int? TransactionCount { get; set; }

    [TextualWhereClause(StringMethod.Contains)]
    public string Username { get; set; }

    [TextualWhereClause(StringMethod.Contains)]
    public string IpAddress { get; set; }
}
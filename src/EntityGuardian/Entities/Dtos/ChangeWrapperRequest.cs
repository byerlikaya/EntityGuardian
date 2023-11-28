namespace EntityGuardian.Entities.Dtos;

public class ChangeWrapperRequest : BaseRequest
{
    [WhereClause("DbContextId")]
    public Guid? ContextId { get; set; }

    [TextualWhereClause("Changes.EntityName", StringMethod.Contains)]
    public string EntityName { get; set; }

    [WhereClause]
    public int? TransactionCount { get; set; }

    [TextualWhereClause(StringMethod.Contains)]
    public string Username { get; set; }

    [TextualWhereClause(StringMethod.Contains)]
    public string IpAddress { get; set; }
}
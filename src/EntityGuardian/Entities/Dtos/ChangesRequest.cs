namespace EntityGuardian.Entities.Dtos;

public class ChangesRequest : BaseRequest
{
    [WhereClause]
    public Guid ChangeWrapperGuid { get; set; }

    [WhereClause]
    public int? Rank { get; set; }

    [TextualWhereClause(StringMethod.Contains)]
    public string TransactionType { get; set; }

    [TextualWhereClause(StringMethod.Contains)]
    public string EntityName { get; set; }
}
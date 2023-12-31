﻿namespace EntityGuardian.Entities.Dtos;

public class ChangesRequest : BaseRequest
{
    [WhereClause]
    public Guid ChangeWrapperGuid { get; set; }

    [TextualWhereClause(StringMethod.Contains)]
    public string TransactionType { get; set; }

    [TextualWhereClause(StringMethod.Contains)]
    public string EntityName { get; set; }
}
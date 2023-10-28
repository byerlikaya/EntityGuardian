using SmartWhere.Attributes;
using System;

namespace EntityGuardian.Entities.Dtos
{
    public class ChangesRequest : BaseRequest
    {
        [WhereClause]
        public Guid ChangeWrapperGuid { get; set; }
    }
}

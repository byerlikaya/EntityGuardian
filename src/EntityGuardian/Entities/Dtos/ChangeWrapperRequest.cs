using SmartWhere.Attributes;
using System;

namespace EntityGuardian.Entities.Dtos
{
    public class ChangeWrapperRequest : BaseRequest
    {
        [WhereClause]
        public Guid? Guid { get; set; }
    }
}

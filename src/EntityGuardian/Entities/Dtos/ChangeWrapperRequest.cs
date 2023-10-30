using SmartWhere.Attributes;
using SmartWhere.Enums;

namespace EntityGuardian.Entities.Dtos
{
    public class ChangeWrapperRequest : BaseRequest
    {
        [TextualWhereClause(StringMethod.Contains)]
        public string TargetName { get; set; }

        [TextualWhereClause(StringMethod.Contains)]
        public string MethodName { get; set; }

        [TextualWhereClause(StringMethod.Contains)]
        public string Username { get; set; }

        [TextualWhereClause(StringMethod.Contains)]
        public string IpAddress { get; set; }
    }
}

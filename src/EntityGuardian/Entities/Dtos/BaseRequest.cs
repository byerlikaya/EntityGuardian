using SmartOrderBy.Dtos;
using SmartWhere.Interfaces;

namespace EntityGuardian.Entities.Dtos
{
    public class BaseRequest : IWhereClause
    {
        public string SearchValue { get; set; }
        public int Start { get; set; }
        public int Max { get; set; }
        public Sorting OrderBy { get; set; }
    }
}

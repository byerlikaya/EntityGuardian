using SmartOrderBy.Dtos;
using SmartWhere.Interfaces;

namespace EntityGuardian.Entities.Results
{
    public class SearcRequest : IWhereClause
    {
        public string SearchValue { get; set; }
        public int Start { get; set; }
        public int Max { get; set; }
        public Sorting OrderBy { get; set; }
    }
}

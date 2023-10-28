using SmartOrderBy.Dtos;
using SmartWhere.Interfaces;
using System;

namespace EntityGuardian.Entities.Results
{
    public class SearcRequest : IWhereClause
    {
        public Guid Guid { get; set; }
        public string SearchValue { get; set; }
        public int Start { get; set; }
        public int Max { get; set; }
        public Sorting OrderBy { get; set; }
    }
}

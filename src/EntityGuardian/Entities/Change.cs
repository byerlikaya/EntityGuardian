using System;

namespace EntityGuardian.Entities
{
    public class Change
    {
        public Guid Guid { get; set; }

        public Guid ChangeWrapperGuid { get; set; }

        public string ActionType { get; set; }

        public string EntityName { get; set; }

        public string OldData { get; set; }

        public string NewData { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
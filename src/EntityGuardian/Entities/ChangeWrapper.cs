using System;
using System.Collections.Generic;

namespace EntityGuardian.Entities
{
    public class ChangeWrapper
    {
        public Guid Guid { get; set; }

        public string Username { get; set; }

        public string IpAddress { get; set; }

        public string TargetName { get; set; }

        public string MethodName { get; set; }

        public List<Change> Changes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory_v2.Model
{
    public class UserLog
    {
        public string Id { get; set; } = string.Empty;
        public string StaffNo { get; set; } = string.Empty;
        public int LogCreated { get; set; }
    }
}
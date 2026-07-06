using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory_v2.Model
{
    public class Activity
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActionTaken { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public int CreatedOn { get; set; }
    }
}
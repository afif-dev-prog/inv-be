using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory_v2.Model
{
    public class MovementCart
    {
        public string Id { get; set; } = string.Empty;
        public string JsonObject { get; set; } = string.Empty;
        public string AssetInvolved { get; set; } = string.Empty;
        public int CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
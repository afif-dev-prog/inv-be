using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory_v2.Model
{
    public class Movement
    {
        public string Id { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string AssetNumber { get; set; } = string.Empty;
        public string AssetId { get; set; } = string.Empty;
        public string OriginLocation { get; set; } = string.Empty;
        public string OriginBranch { get; set; } = string.Empty;
        public string OriginDepartment { get; set; } = string.Empty;
        public string NewLocation { get; set; } = string.Empty;
        public string NewBranch { get; set; } = string.Empty;
        public string NewDepartment { get; set; } = string.Empty;
        public string NewOwnerName { get; set; } = string.Empty;
        public string NewOwnerBranch { get; set; } = string.Empty;
        public string NewOwnerDepartment { get; set; } = string.Empty;
        public string PreviousOwnerName { get; set; } = string.Empty;
        public string PreviousOwnerBranch { get; set; } = string.Empty;
        public string PreviousOwnerDepartment { get; set; } = string.Empty;
        public string MovementStatus { get; set; } = string.Empty;
        public int CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
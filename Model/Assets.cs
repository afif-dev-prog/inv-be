using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory_v2.Model
{
    public class Assets
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AssetNumber { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string PONumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string ReceivingDate { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public string AssetLocation { get; set; } = string.Empty;
        public string AssetBranch { get; set; } = string.Empty;
        public string AssetDepartment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Capacity { get; set; }
        public double PricePerUnit { get; set; }
        public double TotalPrice { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerDepartment { get; set; } = string.Empty;
        public string OwnerBranch { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public int CreatedOn { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public int UpdatedOn { get; set; }
        public List<ChildAsset>? ChildAsset { get; set; }



    }

    public class MappedAssets
    {
        public Assets? Assets { get; set; }
        public List<ChildAsset>? ChildAssets { get; set; }
    }

    public class ChildAsset
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AssetNumber { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string PONumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string ReceivingDate { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public string AssetLocation { get; set; } = string.Empty;
        public string AssetBranch { get; set; } = string.Empty;
        public string AssetDepartment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Capacity { get; set; }
        public double PricePerUnit { get; set; }
        public double TotalPrice { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerDepartment { get; set; } = string.Empty;
        public string OwnerBranch { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public int CreatedOn { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public int UpdatedOn { get; set; }
        public string ParentId { get; set; } = string.Empty;
    }
}
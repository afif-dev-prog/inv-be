using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory_v2.Model
{
    public class Category
    {
        public string Id { get; set; } = string.Empty;
        public string Categories { get; set; } = string.Empty;
        public int CreatedOn { get; set; }
        public int LastUpdateOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
    }

    public class EditCategory
    {
        public string Categories { get; set; } = string.Empty;
        public int CreatedOn { get; set; }
        public int LastUpdateOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
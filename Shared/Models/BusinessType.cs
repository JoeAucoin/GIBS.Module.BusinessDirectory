using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Models;
using System.Collections.Generic;

namespace GIBS.Module.BusinessDirectory.Models
{
    [Table("GIBSBusinessType")]
    public class BusinessType : IAuditable
    {
        [Key]
        public int TypeId { get; set; }
        public int ModuleId { get; set; }
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
        public string ImageURL { get; set; }
        public int SortOrder { get; set; }
        public bool IsNewItem { get; set; }
        public bool IsActive { get; set; }
        public int ParentId { get; set; } // For hierarchical structure

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Slug { get; set; } // For SEO-friendly URLs


        [NotMapped]
        public List<BusinessType> Children { get; set; } = new List<BusinessType>();
    }

}

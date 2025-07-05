using Oqtane.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GIBS.Module.BusinessDirectory.Models
{
    [Table("GIBSBusinessCompany")]
    public class BusinessCompany : IAuditable
    {
        [Key]
        public int CompanyId { get; set; }
        public int ModuleId { get; set; }
        public int TypeId { get; set; } // Foreign key to BusinessType
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int SortOrder { get; set; }
        public bool IsNewItem { get; set; }
        public bool IsActive { get; set; }
        public double Latitude { get; set; } // For geolocation
        public double Longitude { get; set; } // For geolocation
        // Auditable properties
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Slug { get; set; } // For SEO-friendly URLs


        [NotMapped]
        public string TypeName { get; set; } // Not mapped, for display only

        [NotMapped]
        public string TypeDescription { get; set; } // Not mapped, for display only

        [InverseProperty("BusinessCompany")]
        public virtual ICollection<BusinessToAttribute> BusinessToAttribute { get; set; } = new List<BusinessToAttribute>();
    }
}

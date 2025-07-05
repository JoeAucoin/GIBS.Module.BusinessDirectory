using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Oqtane.Models;

namespace GIBS.Module.BusinessDirectory.Models
{
    [Table("GIBSBusinessToAttribute")]
    public class BusinessToAttribute : IAuditable
    {
        [Key]
        public int BusinessAttributeId { get; set; }
        public int CompanyId { get; set; }
        public int AttributeId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

        [ForeignKey("CompanyId")]
        [JsonIgnore] // Keep this to prevent cycles, but allows attributes to be returned in company JSON
        public virtual BusinessCompany BusinessCompany { get; set; }

        [ForeignKey("AttributeId")]
        public virtual BAttribute BAttribute { get; set; }
    }
}
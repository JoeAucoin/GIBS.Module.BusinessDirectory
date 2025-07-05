using System.Collections.Generic;

namespace GIBS.Module.BusinessDirectory.Models
{
    public class BusinessDirectoryExportDto
    {
        public List<BusinessType> BusinessTypes { get; set; }
        public List<BusinessCompany> BusinessCompanies { get; set; }
        public List<BAttribute> BAttributes { get; set; }
        public List<BusinessToAttribute> BusinessToAttributes { get; set; }
    }
}
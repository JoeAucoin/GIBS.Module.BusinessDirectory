using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GIBS.Module.BusinessDirectory.Models;


namespace GIBS.Module.BusinessDirectory.Repository
{
    public interface IBusinessCompanyRepository
    {
    //    IEnumerable<BusinessCompany> GetBusinessCompanies(int moduleId);
        Task<BusinessCompany> GetBusinessCompanyAsync(int companyId, int moduleId);
        Task<BusinessCompany> AddBusinessCompanyAsync(BusinessCompany businessCompany);
        Task<BusinessCompany> UpdateBusinessCompanyAsync(BusinessCompany businessCompany);
        Task DeleteBusinessCompanyAsync(int companyId, int module);
        

        Task<List<BusinessCompany>> GetBusinessCompaniesAsync(int moduleId);

        Task<List<BusinessCompany>> GetBusinessCompaniesByTypeAsync(int typeId, int moduleId);

        Task<List<BusinessCompany>> GetBusinessCompaniesByIsNewItemAsync(int moduleId);
    }
}

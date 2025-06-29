using GIBS.Module.BusinessDirectory.Interfaces;
using GIBS.Module.BusinessDirectory.Models;
using GIBS.Module.BusinessDirectory.Repository;
using Oqtane.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GIBS.Module.BusinessDirectory.Services
{
    public class BusinessCompanyService : IBusinessCompanyService, IService
    {
        private readonly IBusinessCompanyService _repository;

        public BusinessCompanyService(IBusinessCompanyService repository)
        {
            _repository = repository;
        }

        public async Task<List<BusinessCompany>> GetBusinessCompaniesAsync(int ModuleId)
        {
            // Await the task before calling ToList to resolve the CS1061 error
            var companies = await _repository.GetBusinessCompaniesAsync(ModuleId);
            return companies.ToList();
        }

        public async Task<BusinessCompany> GetBusinessCompanyAsync(int CompanyId, int ModuleId)
        {
            return await _repository.GetBusinessCompanyAsync(CompanyId, ModuleId);
        }

        public async Task<BusinessCompany> AddBusinessCompanyAsync(BusinessCompany businessCompany)
        {
            return await _repository.AddBusinessCompanyAsync(businessCompany);
        }

        public async Task<BusinessCompany> UpdateBusinessCompanyAsync(BusinessCompany businessCompany)
        {
            return await _repository.UpdateBusinessCompanyAsync(businessCompany);
        }

        public async Task DeleteBusinessCompanyAsync(int CompanyId, int ModuleId)
        {
            await _repository.DeleteBusinessCompanyAsync(CompanyId, ModuleId);
        }

        public async Task<List<BusinessCompany>> GetBusinessCompaniesByTypeAsync(int typeId, int moduleId)
        {
            return await _repository.GetBusinessCompaniesByTypeAsync(typeId, moduleId);
        }

        public async Task<List<BusinessCompany>> GetBusinessCompaniesByIsNewItemAsync(int moduleId)
        {
            return await _repository.GetBusinessCompaniesByIsNewItemAsync(moduleId);
        }

        public Task<string> UploadImageAsync(byte[] fileBytes, string fileName)
        {
            // Not used on server side, but required by interface
            throw new System.NotImplementedException();
        }
    }
}

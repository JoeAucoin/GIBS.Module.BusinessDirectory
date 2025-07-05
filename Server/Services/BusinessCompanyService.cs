using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Security;
using Oqtane.Shared;
using GIBS.Module.BusinessDirectory.Repository;
using GIBS.Module.BusinessDirectory.Models;
using System;

namespace GIBS.Module.BusinessDirectory.Services
{
    public class ServerBusinessCompanyService : IBusinessCompanyService
    {
        private readonly IBusinessCompanyRepository _businessCompanyRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;

        public ServerBusinessCompanyService(IBusinessCompanyRepository businessCompanyRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor)
        {
            _businessCompanyRepository = businessCompanyRepository;
            _userPermissions = userPermissions;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();
        }

        public Task<List<BusinessCompany>> GetBusinessCompaniesAsync(int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                return Task.FromResult(_businessCompanyRepository.GetBusinessCompaniesAsync(moduleId).Result.ToList());
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany Get Attempt {ModuleId}", moduleId);
                return null;
            }
        }

        public Task<BusinessCompany> GetBusinessCompanyAsync(int companyId, int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                return Task.FromResult(_businessCompanyRepository.GetBusinessCompanyAsync(companyId, moduleId).Result);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany Get Attempt {CompanyId} {ModuleId}", companyId, moduleId);
                return null;
            }
        }

        public Task<BusinessCompany> AddBusinessCompanyAsync(BusinessCompany businessCompany)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, businessCompany.ModuleId, PermissionNames.Edit))
            {
                businessCompany = _businessCompanyRepository.AddBusinessCompanyAsync(businessCompany).Result;
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "BusinessCompany Added {BusinessCompany}", businessCompany);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany Add Attempt {BusinessCompany}", businessCompany);
                businessCompany = null;
            }
            return Task.FromResult(businessCompany);
        }

        public Task<BusinessCompany> UpdateBusinessCompanyAsync(BusinessCompany businessCompany)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, businessCompany.ModuleId, PermissionNames.Edit))
            {
                businessCompany = _businessCompanyRepository.UpdateBusinessCompanyAsync(businessCompany).Result;
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "BusinessCompany Updated {BusinessCompany}", businessCompany);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany Update Attempt {BusinessCompany}", businessCompany);
                businessCompany = null;
            }
            return Task.FromResult(businessCompany);
        }

        public Task DeleteBusinessCompanyAsync(int companyId, int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.Edit))
            {
                _businessCompanyRepository.DeleteBusinessCompanyAsync(companyId, moduleId);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "BusinessCompany Deleted {CompanyId}", companyId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany Delete Attempt {CompanyId} {ModuleId}", companyId, moduleId);
            }
            return Task.CompletedTask;
        }

        public Task<List<BusinessCompany>> GetBusinessCompaniesByTypeAsync(int typeId, int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                return Task.FromResult(_businessCompanyRepository.GetBusinessCompaniesByTypeAsync(typeId, moduleId).Result.ToList());
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany GetByType Attempt {TypeId} {ModuleId}", typeId, moduleId);
                return null;
            }
        }

        public Task<List<BusinessCompany>> GetBusinessCompaniesByIsNewItemAsync(int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                return Task.FromResult(_businessCompanyRepository.GetBusinessCompaniesByIsNewItemAsync(moduleId).Result.ToList());
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany GetByIsNewItem Attempt {ModuleId}", moduleId);
                return null;
            }
        }

        public Task<string> UploadImageAsync(byte[] fileBytes, string fileName)
        {
            // Not used on server side, but required by interface
            throw new System.NotImplementedException("Image upload is not implemented on the server side.");
        }

        public async Task UpdateCompanyAttributesAsync(int companyId, int moduleId, List<int> attributeIds)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.Edit))
            {
                await _businessCompanyRepository.UpdateCompanyAttributesAsync(companyId, moduleId, attributeIds);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Company Attributes Updated {CompanyId} {ModuleId}", companyId, moduleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized UpdateCompanyAttributes Attempt {CompanyId} {ModuleId}", companyId, moduleId);
                throw new UnauthorizedAccessException("Unauthorized access to update company attributes");
            }
        }

        public async Task<List<Models.BusinessCompany>> GetCompanyAttributesAsync(int companyId, int moduleId)
        {
            return await _businessCompanyRepository.GetCompanyAttributesAsync(companyId, moduleId);
        }
    }
}
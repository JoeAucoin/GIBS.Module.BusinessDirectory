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

namespace GIBS.Module.BusinessDirectory.Services
{
    public class ServerBusinessDirectoryService : IBusinessDirectoryService
    {
        private readonly IBusinessDirectoryRepository _BusinessDirectoryRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;

        public ServerBusinessDirectoryService(IBusinessDirectoryRepository BusinessDirectoryRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor)
        {
            _BusinessDirectoryRepository = BusinessDirectoryRepository;
            _userPermissions = userPermissions;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();
        }

        public Task<List<Models.BusinessType>> GetBusinessDirectorysAsync(int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                return Task.FromResult(_BusinessDirectoryRepository.GetBusinessDirectorys(ModuleId).ToList());
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessDirectory Get Attempt {ModuleId}", ModuleId);
                return null;
            }
        }

        public Task<Models.BusinessType> GetBusinessDirectoryAsync(int BusinessDirectoryId, int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                return Task.FromResult(_BusinessDirectoryRepository.GetBusinessDirectory(BusinessDirectoryId));
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessDirectory Get Attempt {BusinessDirectoryId} {ModuleId}", BusinessDirectoryId, ModuleId);
                return null;
            }
        }

        public Task<Models.BusinessType> AddBusinessDirectoryAsync(Models.BusinessType BusinessDirectory)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, BusinessDirectory.ModuleId, PermissionNames.Edit))
            {
                BusinessDirectory = _BusinessDirectoryRepository.AddBusinessDirectory(BusinessDirectory);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "BusinessDirectory Added {BusinessDirectory}", BusinessDirectory);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessDirectory Add Attempt {BusinessDirectory}", BusinessDirectory);
                BusinessDirectory = null;
            }
            return Task.FromResult(BusinessDirectory);
        }

        public Task<Models.BusinessType> UpdateBusinessDirectoryAsync(Models.BusinessType BusinessDirectory)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, BusinessDirectory.ModuleId, PermissionNames.Edit))
            {
                BusinessDirectory = _BusinessDirectoryRepository.UpdateBusinessDirectory(BusinessDirectory);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "BusinessDirectory Updated {BusinessDirectory}", BusinessDirectory);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessDirectory Update Attempt {BusinessDirectory}", BusinessDirectory);
                BusinessDirectory = null;
            }
            return Task.FromResult(BusinessDirectory);
        }

        public Task DeleteBusinessDirectoryAsync(int BusinessDirectoryId, int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.Edit))
            {
                _BusinessDirectoryRepository.DeleteBusinessDirectory(BusinessDirectoryId);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "BusinessDirectory Deleted {BusinessDirectoryId}", BusinessDirectoryId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessDirectory Delete Attempt {BusinessDirectoryId} {ModuleId}", BusinessDirectoryId, ModuleId);
            }
            return Task.CompletedTask;
        }
    }
}

using GIBS.Module.BusinessDirectory.Repository;
using GIBS.Module.BusinessDirectory.Models;
using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Security;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GIBS.Module.BusinessDirectory.Services
{
    public class ServerBAttributeService : IBAttributeService, ITransientService  // Changed from IService to ITransientService
    {
        private readonly IBAttributeRepository _attributeRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogManager _logger;
        private readonly IAliasAccessor _aliasAccessor;  // Changed from ITenantManager to IAliasAccessor

        public ServerBAttributeService(IBAttributeRepository attributeRepository, IUserPermissions userPermissions, IHttpContextAccessor accessor, ILogManager logger, IAliasAccessor aliasAccessor)  // Updated constructor
        {
            _attributeRepository = attributeRepository;
            _userPermissions = userPermissions;
            _accessor = accessor;
            _logger = logger;
            _aliasAccessor = aliasAccessor;  // Updated assignment
        }

        public async Task<IEnumerable<BAttribute>> GetAttributesAsync(int moduleId)
        {
            var alias = _aliasAccessor.Alias;  // Changed from _tenantManager.GetAlias() to _aliasAccessor.Alias
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                var attributes = await _attributeRepository.GetAttributesAsync(moduleId);
                return attributes;
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized GetAttributesAsync Request For Module {ModuleId}", moduleId);
                return null;
            }
        }

        // Update all other methods the same way - replace _tenantManager.GetAlias() with _aliasAccessor.Alias
        public async Task<BAttribute> GetAttributeAsync(int attributeId)
        {
            var alias = _aliasAccessor.Alias;
            var attribute = await _attributeRepository.GetAttributeAsync(attributeId);
            if (attribute != null && _userPermissions.IsAuthorized(_accessor.HttpContext.User, alias.SiteId, EntityNames.Module, attribute.ModuleId, PermissionNames.View))
            {
                return attribute;
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized GetAttributeAsync Request For Attribute {AttributeId}", attributeId);
                return null;
            }
        }

        public async Task<BAttribute> AddAttributeAsync(BAttribute attribute)
        {
            var alias = _aliasAccessor.Alias;
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, alias.SiteId, EntityNames.Module, attribute.ModuleId, PermissionNames.Edit))
            {
                return await _attributeRepository.AddAttributeAsync(attribute);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized AddAttributeAsync Request For Module {ModuleId}", attribute.ModuleId);
                return null;
            }
        }

        public async Task<BAttribute> UpdateAttributeAsync(BAttribute attribute)
        {
            var alias = _aliasAccessor.Alias;
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, alias.SiteId, EntityNames.Module, attribute.ModuleId, PermissionNames.Edit))
            {
                return await _attributeRepository.UpdateAttributeAsync(attribute);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized UpdateAttributeAsync Request For Module {ModuleId}", attribute.ModuleId);
                return null;
            }
        }

        public async Task DeleteAttributeAsync(int attributeId)
        {
            var alias = _aliasAccessor.Alias;
            var attribute = await _attributeRepository.GetAttributeAsync(attributeId);
            if (attribute != null && _userPermissions.IsAuthorized(_accessor.HttpContext.User, alias.SiteId, EntityNames.Module, attribute.ModuleId, PermissionNames.Edit))
            {
                await _attributeRepository.DeleteAttributeAsync(attributeId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized DeleteAttributeAsync Request For Attribute {AttributeId}", attributeId);
            }
        }
    }
}
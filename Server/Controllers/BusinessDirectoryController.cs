using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using GIBS.Module.BusinessDirectory.Services;
using Oqtane.Controllers;
using System.Net;
using System.Threading.Tasks;

namespace GIBS.Module.BusinessDirectory.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class BusinessDirectoryController : ModuleControllerBase
    {
        private readonly IBusinessDirectoryService _BusinessDirectoryService;

        public BusinessDirectoryController(IBusinessDirectoryService BusinessDirectoryService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _BusinessDirectoryService = BusinessDirectoryService;
        }

        // GET: api/<controller>?moduleid=x
        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<Models.BusinessType>> Get(string moduleid)
        {
            int ModuleId;
            if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
            {
                return await _BusinessDirectoryService.GetBusinessDirectorysAsync(ModuleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessDirectory Get Attempt {ModuleId}", moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<Models.BusinessType> Get(int id, int moduleid)
        {
            Models.BusinessType BusinessDirectory = await _BusinessDirectoryService.GetBusinessDirectoryAsync(id, moduleid);
            if (BusinessDirectory != null && IsAuthorizedEntityId(EntityNames.Module, BusinessDirectory.ModuleId))
            {
                return BusinessDirectory;
            }
            else
            { 
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessDirectory Get Attempt {BusinessDirectoryId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.BusinessType> Post([FromBody] Models.BusinessType BusinessDirectory)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, BusinessDirectory.ModuleId))
            {
                BusinessDirectory = await _BusinessDirectoryService.AddBusinessDirectoryAsync(BusinessDirectory);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessDirectory Post Attempt {BusinessDirectory}", BusinessDirectory);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                BusinessDirectory = null;
            }
            return BusinessDirectory;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.BusinessType> Put(int id, [FromBody] Models.BusinessType BusinessDirectory)
        {
            if (ModelState.IsValid && BusinessDirectory.TypeId == id && IsAuthorizedEntityId(EntityNames.Module, BusinessDirectory.ModuleId))
            {
                BusinessDirectory = await _BusinessDirectoryService.UpdateBusinessDirectoryAsync(BusinessDirectory);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessDirectory Put Attempt {BusinessDirectory}", BusinessDirectory);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                BusinessDirectory = null;
            }
            return BusinessDirectory;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task Delete(int id, int moduleid)
        {
            Models.BusinessType BusinessDirectory = await _BusinessDirectoryService.GetBusinessDirectoryAsync(id, moduleid);
            if (BusinessDirectory != null && IsAuthorizedEntityId(EntityNames.Module, BusinessDirectory.ModuleId))
            {
                await _BusinessDirectoryService.DeleteBusinessDirectoryAsync(id, BusinessDirectory.ModuleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessDirectory Delete Attempt {TypeId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}

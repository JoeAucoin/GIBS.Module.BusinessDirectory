using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Controllers;
using GIBS.Module.BusinessDirectory.Models;
using GIBS.Module.BusinessDirectory.Services;
using System.Threading.Tasks;
using System.Net;

namespace GIBS.Module.BusinessDirectory.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class BAttributeController : ModuleControllerBase
    {
        private readonly IBAttributeService _attributeService;

        public BAttributeController(IBAttributeService attributeService, ILogManager logger, IHttpContextAccessor accessor)
            : base(logger, accessor)
        {
            _attributeService = attributeService;
        }

        // GET: api/<controller>?moduleid=x
        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<BAttribute>> Get(string moduleid)
        {
            int ModuleId;
            if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
            {
                return await _attributeService.GetAttributesAsync(ModuleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Attribute Get Attempt {ModuleId}", moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<BAttribute> Get(int id)
        {
            BAttribute attribute = await _attributeService.GetAttributeAsync(id);
            if (attribute != null)
            {
                return attribute;
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Attribute Get Attempt {AttributeId}", id);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<BAttribute> Post([FromBody] BAttribute attribute)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, attribute.ModuleId))
            {
                attribute = await _attributeService.AddAttributeAsync(attribute);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Attribute Added {Attribute}", attribute);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Attribute Post Attempt {Attribute}", attribute);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                attribute = null;
            }
            return attribute;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<BAttribute> Put(int id, [FromBody] BAttribute attribute)
        {
            if (ModelState.IsValid && attribute.AttributeId == id && IsAuthorizedEntityId(EntityNames.Module, attribute.ModuleId))
            {
                attribute = await _attributeService.UpdateAttributeAsync(attribute);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Attribute Updated {Attribute}", attribute);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Attribute Put Attempt {Attribute}", attribute);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                attribute = null;
            }
            return attribute;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task Delete(int id)
        {
            BAttribute attribute = await _attributeService.GetAttributeAsync(id);
            if (attribute != null && IsAuthorizedEntityId(EntityNames.Module, attribute.ModuleId))
            {
                await _attributeService.DeleteAttributeAsync(id);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Attribute Deleted {AttributeId}", id);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Attribute Delete Attempt {AttributeId}", id);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}

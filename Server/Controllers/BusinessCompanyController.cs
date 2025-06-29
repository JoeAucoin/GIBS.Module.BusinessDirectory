using GIBS.Module.BusinessDirectory.Models;
using GIBS.Module.BusinessDirectory.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Controllers;
using Oqtane.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using Oqtane.Shared;
using Oqtane.Enums;

namespace GIBS.Module.BusinessDirectory.Server.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class BusinessCompanyController : ModuleControllerBase
    {
        private readonly IBusinessCompanyRepository _businessCompanyRepository;
        private readonly IWebHostEnvironment _env;
   //     private readonly IFolderRepository _folders;

        public BusinessCompanyController(
            IBusinessCompanyRepository businessCompanyRepository,
            ILogManager logger,
            IHttpContextAccessor accessor,
            IWebHostEnvironment env
        ) : base(logger, accessor)
        {
            _businessCompanyRepository = businessCompanyRepository;
            _env = env;
        }

        // GET: api/BusinessCompany?moduleid=x
        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<BusinessCompany>> Get(string moduleid)
        {
           
            int moduleId;
            if (int.TryParse(moduleid, out moduleId) && IsAuthorizedEntityId(EntityNames.Module, moduleId))
            {
                return await _businessCompanyRepository.GetBusinessCompaniesAsync(moduleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany Get Attempt {ModuleId}", moduleid);
                return null;
            }
        }

        // GET: api/BusinessCompany/5/34
        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<BusinessCompany> Get(int id, int moduleid)
        {
            var businessCompany = await _businessCompanyRepository.GetBusinessCompanyAsync(id, moduleid);
            if (businessCompany != null && IsAuthorizedEntityId(EntityNames.Module, businessCompany.ModuleId))
            {
                return businessCompany;
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany Get Attempt {BusinessCompanyId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }
        }

        // POST: api/BusinessCompany
        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<ActionResult<BusinessCompany>> Post([FromBody] BusinessCompany businessCompany)
        {
            if (businessCompany == null || !IsAuthorizedEntityId(EntityNames.Module, businessCompany.ModuleId))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany Post Attempt {ModuleId}", businessCompany?.ModuleId);
                return Forbid();
            }

            var created = await _businessCompanyRepository.AddBusinessCompanyAsync(businessCompany);
            return CreatedAtAction(nameof(Get), new { id = created.CompanyId, moduleid = created.ModuleId }, created);
        }

        // PUT: api/BusinessCompany/5
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<ActionResult<BusinessCompany>> Put(int id, [FromBody] BusinessCompany businessCompany)
        {
            if (businessCompany == null || businessCompany.CompanyId != id || !IsAuthorizedEntityId(EntityNames.Module, businessCompany.ModuleId))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany Put Attempt {BusinessCompanyId} {ModuleId}", id, businessCompany?.ModuleId);
                return Forbid();
            }

            var updated = await _businessCompanyRepository.UpdateBusinessCompanyAsync(businessCompany);
            return Ok(updated);
        }

        // DELETE: api/BusinessCompany/5/34
        [HttpDelete("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<IActionResult> Delete(int id, int moduleid)
        {
            var businessCompany = await _businessCompanyRepository.GetBusinessCompanyAsync(id, moduleid);
            if (businessCompany == null || !IsAuthorizedEntityId(EntityNames.Module, businessCompany.ModuleId))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized BusinessCompany Delete Attempt {BusinessCompanyId} {ModuleId}", id, moduleid);
                return Forbid();
            }

            await _businessCompanyRepository.DeleteBusinessCompanyAsync(id, moduleid);
            return NoContent();
        }

        [HttpGet("bytype/{typeId}/{moduleId}")]
        public async Task<ActionResult<List<BusinessCompany>>> GetBusinessCompaniesByType(int typeId, int moduleId)
        {
            var companies = await _businessCompanyRepository.GetBusinessCompaniesByTypeAsync(typeId, moduleId);
            return Ok(companies);
        }

        [HttpGet("isnew/{moduleId}")]
        public async Task<ActionResult<List<BusinessCompany>>> GetBusinessCompaniesByIsNewItem(int moduleId)
        {
            var companies = await _businessCompanyRepository.GetBusinessCompaniesByIsNewItemAsync(moduleId);
            return Ok(companies);
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (_env == null)
                return StatusCode(500, "WebHostEnvironment is not available.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "business-images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var imageUrl = $"{baseUrl}/uploads/business-images/{fileName}";
            return Ok(imageUrl);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] string folder, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!folder.Contains(":\\"))
            {
                folder = folder.Replace("/", "\\");
                if (folder.StartsWith("\\")) folder = folder.Substring(1);
                folder = Path.Combine(_env.WebRootPath, folder);
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filePath = Path.Combine(folder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            await MergeFile(folder, file.FileName);
            return Ok();
        }

        private async Task MergeFile(string folder, string filename)
        {
            // parse the filename which is in the format of filename.ext.part_x_y
            string token = ".part_";
            string parts = Path.GetExtension(filename).Replace(token, ""); // returns "x_y"
            int totalparts = int.Parse(parts.Substring(parts.IndexOf("_") + 1));
            filename = filename.Substring(0, filename.IndexOf(token)); // base filename
            string[] fileparts = Directory.GetFiles(folder, filename + token + "*"); // list of all file parts

            // if all of the file parts exist (note that file parts can arrive out of order)
            if (fileparts.Length == totalparts)
            {
                // merge file parts
                bool success = true;
                using (var stream = new FileStream(Path.Combine(folder, filename), FileMode.Create))
                {
                    foreach (string filepart in fileparts)
                    {
                        try
                        {
                            using (FileStream chunk = new FileStream(filepart, FileMode.Open))
                            {
                                await chunk.CopyToAsync(stream);
                            }
                        }
                        catch
                        {
                            success = false;
                        }
                    }
                }

                // delete file parts
                if (success)
                {
                    foreach (string filepart in fileparts)
                    {
                        System.IO.File.Delete(filepart);
                    }
                }
            }

            // clean up file parts which are more than 2 hours old
            fileparts = Directory.GetFiles(folder, "*" + token + "*");
            foreach (string filepart in fileparts)
            {
                DateTime createddate = System.IO.File.GetCreationTime(filepart);
                if (createddate < DateTime.Now.AddHours(-2))
                {
                    System.IO.File.Delete(filepart);
                }
            }
        }
    }
}
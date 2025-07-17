using GIBS.Module.BusinessDirectory.Models;
using GIBS.Module.BusinessDirectory.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Controllers;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Services;
using Oqtane.Shared;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace GIBS.Module.BusinessDirectory.Server.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class BusinessCompanyController : ModuleControllerBase
    {
        private readonly IBusinessCompanyRepository _businessCompanyRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IFileRepository _files;
        private readonly IImageService _imageService;

        public BusinessCompanyController(
            IBusinessCompanyRepository businessCompanyRepository,
            ILogManager logger,
            IHttpContextAccessor accessor,
            IWebHostEnvironment env,
            IFileRepository files,
            IImageService imageService
        ) : base(logger, accessor)
        {
            _businessCompanyRepository = businessCompanyRepository;
            _env = env;
            _files = files;
            _imageService = imageService;
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
                return businessCompany; // Now includes attributes
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
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (_env == null)
                return StatusCode(500, "WebHostEnvironment is not available.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "business-images");
            _logger.Log(LogLevel.Information, this, LogFunction.Create, "Uploads folder: {UploadsFolder}", uploadsFolder);
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

        [HttpPost("resize-image")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<ActionResult<Oqtane.Models.File>> ResizeImage([FromBody] ResizeRequest request)
        {
            var file = _files.GetFile(request.FileId);
            if (file == null || !IsAuthorizedEntityId(EntityNames.Module, request.ModuleId))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Image Resize Attempt for FileId {FileId}", request.FileId);
                return Forbid();
            }

            string originalFilePath = null;
            string tempFilePath = null;

            try
            {
                originalFilePath = _files.GetFilePath(file);
                if (System.IO.File.Exists(originalFilePath))
                {
                    // 1. Create a temporary path for the resized image
                    tempFilePath = Path.ChangeExtension(originalFilePath, ".tmp" + Path.GetExtension(originalFilePath));

                    // 2. Use the IImageService to create the resized image at the temporary path
                    var resizedImagePath = _imageService.CreateImage(originalFilePath, request.Width, request.Height, "medium", "center", "white", "", file.Extension, tempFilePath);

                    if (string.IsNullOrEmpty(resizedImagePath) || !System.IO.File.Exists(resizedImagePath))
                    {
                        throw new Exception("Image resizing failed during temporary file creation.");
                    }

                    // 3. Update file metadata
                    var fileInfo = new FileInfo(resizedImagePath);
                    using (var image = await Image.LoadAsync(resizedImagePath))
                    {
                        file.ImageWidth = image.Width;
                        file.ImageHeight = image.Height;
                    }
                    file.Size = (int)fileInfo.Length;

                    // 4. The file is now resized and metadata is updated, but we need to replace the original file
                    // At this point, all handles to the original file should be closed.
                    System.IO.File.Delete(originalFilePath);
                    System.IO.File.Move(resizedImagePath, originalFilePath);

                    _files.UpdateFile(file);

                    _logger.Log(LogLevel.Information, this, LogFunction.Update, "Image Resized Successfully {FileId}", request.FileId);
                    return Ok(file);
                }
                else
                {
                    _logger.Log(LogLevel.Error, this, LogFunction.Read, "File Not Found For Resizing {FileId}", request.FileId);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Clean up temporary file if it exists
                if (!string.IsNullOrEmpty(tempFilePath) && System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }

                _logger.Log(LogLevel.Error, this, LogFunction.Update, ex, "Error Resizing Image {FileId}", request.FileId);
                return StatusCode(500, "An error occurred while resizing the image.");
            }
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

        [HttpPost("UpdateCompanyAttributes")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<IActionResult> UpdateCompanyAttributes([FromBody] UpdateCompanyAttributesRequest request)
        {
            if (request == null) return BadRequest("Request cannot be null");

            if (!IsAuthorizedEntityId(EntityNames.Module, request.ModuleId))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized UpdateCompanyAttributes Attempt {CompanyId} {ModuleId}", request.CompanyId, request.ModuleId);
                return Forbid();
            }

            await _businessCompanyRepository.UpdateCompanyAttributesAsync(request.CompanyId, request.ModuleId, request.AttributeIds);
            return Ok();
        }


        [HttpGet("companyattributes/{companyId}/{moduleId}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IActionResult> GetCompanyAttributes(int companyId, int moduleId)
        {
            if (!IsAuthorizedEntityId(EntityNames.Module, moduleId))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized GetCompanyAttributes Attempt {CompanyId} {ModuleId}", companyId, moduleId);
                return Forbid();
            }

            try
            {
                var companies = await _businessCompanyRepository.GetCompanyAttributesAsync(companyId, moduleId);
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Read, ex, "Error getting company attributes for {CompanyId} {ModuleId}", companyId, moduleId);
                return StatusCode(500, "Error retrieving company attributes");
            }
        }


        public class UpdateCompanyAttributesRequest
        {
            public int CompanyId { get; set; }
            public int ModuleId { get; set; }
            public List<int> AttributeIds { get; set; }
        }

        public class ResizeRequest
        {
            public int FileId { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int ModuleId { get; set; }
        }
    }
}
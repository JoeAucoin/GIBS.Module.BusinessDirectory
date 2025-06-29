using GIBS.Module.BusinessDirectory.Interfaces;

using Oqtane.Enums;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GIBS.Module.BusinessDirectory.Services
{
    public class BusinessCompanyService : ServiceBase, IBusinessCompanyService, IService
    {
        private readonly HttpClient _httpClient;
        public BusinessCompanyService(HttpClient http, SiteState siteState) : base(http, siteState)
        {
            _httpClient = http ?? throw new ArgumentNullException(nameof(http), "HttpClient is not initialized.");
        }

        private string Apiurl => CreateApiUrl("BusinessCompany");

        public async Task<List<Models.BusinessCompany>> GetBusinessCompaniesAsync(int ModuleId)
        {
            List<Models.BusinessCompany> BusinessCompanies = await GetJsonAsync<List<Models.BusinessCompany>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId), Enumerable.Empty<Models.BusinessCompany>().ToList());
            return BusinessCompanies.OrderBy(item => item.CompanyName).ToList();
        }

        public async Task<Models.BusinessCompany> GetBusinessCompanyAsync(int CompanyId, int ModuleId)
        {
            return await GetJsonAsync<Models.BusinessCompany>(CreateAuthorizationPolicyUrl($"{Apiurl}/{CompanyId}/{ModuleId}", EntityNames.Module, ModuleId));
        }

        public async Task<Models.BusinessCompany> AddBusinessCompanyAsync(Models.BusinessCompany BusinessCompany)
        {
            return await PostJsonAsync<Models.BusinessCompany>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, BusinessCompany.ModuleId), BusinessCompany);
        }

        public async Task<Models.BusinessCompany> UpdateBusinessCompanyAsync(Models.BusinessCompany BusinessCompany)
        {
            return await PutJsonAsync<Models.BusinessCompany>(CreateAuthorizationPolicyUrl($"{Apiurl}/{BusinessCompany.CompanyId}", EntityNames.Module, BusinessCompany.ModuleId), BusinessCompany);
        }

        public async Task DeleteBusinessCompanyAsync(int CompanyId, int ModuleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{CompanyId}/{ModuleId}", EntityNames.Module, ModuleId));
        }

        public async Task<List<Models.BusinessCompany>> GetBusinessCompaniesByTypeAsync(int typeId, int moduleId)
        {
            return await GetJsonAsync<List<Models.BusinessCompany>>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/bytype/{typeId}/{moduleId}", EntityNames.Module, moduleId),
                Enumerable.Empty<Models.BusinessCompany>().ToList());
        }

        public async Task<List<Models.BusinessCompany>> GetBusinessCompaniesByIsNewItemAsync(int moduleId)
        {
            return await GetJsonAsync<List<Models.BusinessCompany>>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/isnew/{moduleId}", EntityNames.Module, moduleId),
                Enumerable.Empty<Models.BusinessCompany>().ToList());
        }

        public async Task<string> UploadImageAsync(byte[] fileBytes, string fileName)
        {
            if (_httpClient == null)
                throw new InvalidOperationException("_httpClient is not initialized.");

            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            content.Add(fileContent, "file", fileName);

            var response = await _httpClient.PostAsync("api/BusinessCompany/upload-image", content);
            if (response == null)
                throw new InvalidOperationException("No response from server.");

            if (response.IsSuccessStatusCode)
            {
                var imageUrl = await response.Content.ReadAsStringAsync();
                return imageUrl?.Trim('"'); // Remove quotes if returned as JSON string
            }
            return null;
        }
    }
}

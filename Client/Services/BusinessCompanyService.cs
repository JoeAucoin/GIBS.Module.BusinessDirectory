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
            // This will now include BusinessToAttribute and BAttribute in the deserialized object
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

        public async Task UpdateCompanyAttributesAsync(int companyId, int moduleId, List<int> attributeIds)
        {
            var request = new
            {
                CompanyId = companyId,
                ModuleId = moduleId,
                AttributeIds = attributeIds
            };
            await _httpClient.PostAsJsonAsync($"{Apiurl}/UpdateCompanyAttributes", request);
        }

        public async Task<List<Models.BusinessCompany>> GetCompanyAttributesAsync(int companyId, int moduleId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{Apiurl}/companyattributes/{companyId}/{moduleId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(content) || content.StartsWith("<"))
                    {
                        // Response is HTML instead of JSON, likely an error page
                        throw new InvalidOperationException("Server returned HTML instead of JSON data");
                    }

                    return await response.Content.ReadFromJsonAsync<List<Models.BusinessCompany>>();
                }
                else
                {
                    throw new HttpRequestException($"API call failed with status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Log the error and return empty list
             //   Console.WriteLine($"Error in GetCompanyAttributesAsync: {ex.Message}");
                return new List<Models.BusinessCompany>();
            }
        }

        public async Task<Oqtane.Models.File> ResizeImageAsync(int fileId, int width, int height, int moduleId)
        {
            var request = new { FileId = fileId, Width = width, Height = height, ModuleId = moduleId };
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/resize-image", EntityNames.Module, moduleId);

            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Oqtane.Models.File>();
            }
            catch (Exception ex)
            {
                // Log or handle exceptions as needed
                Console.WriteLine($"Error resizing image: {ex.Message}");
                return null;
            }
        }

    }
}

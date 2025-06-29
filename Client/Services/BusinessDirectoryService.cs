using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GIBS.Module.BusinessDirectory.Services
{
    public class BusinessDirectoryService : ServiceBase, IBusinessDirectoryService
    {
        public BusinessDirectoryService(HttpClient http, SiteState siteState) : base(http, siteState) { }

        private string Apiurl => CreateApiUrl("BusinessDirectory");

        public async Task<List<Models.BusinessType>> GetBusinessDirectorysAsync(int ModuleId)
        {
            List<Models.BusinessType> BusinessDirectorys = await GetJsonAsync<List<Models.BusinessType>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId), Enumerable.Empty<Models.BusinessType>().ToList());
            return BusinessDirectorys.OrderBy(item => item.TypeName).ToList();
        }

        public async Task<Models.BusinessType> GetBusinessDirectoryAsync(int TypeId, int ModuleId)
        {
            return await GetJsonAsync<Models.BusinessType>(CreateAuthorizationPolicyUrl($"{Apiurl}/{TypeId}/{ModuleId}", EntityNames.Module, ModuleId));
        }

        public async Task<Models.BusinessType> AddBusinessDirectoryAsync(Models.BusinessType BusinessDirectory)
        {
            return await PostJsonAsync<Models.BusinessType>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, BusinessDirectory.ModuleId), BusinessDirectory);
        }

        public async Task<Models.BusinessType> UpdateBusinessDirectoryAsync(Models.BusinessType BusinessDirectory)
        {
            return await PutJsonAsync<Models.BusinessType>(CreateAuthorizationPolicyUrl($"{Apiurl}/{BusinessDirectory.TypeId}", EntityNames.Module, BusinessDirectory.ModuleId), BusinessDirectory);
        }

        public async Task DeleteBusinessDirectoryAsync(int TypeId, int ModuleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{TypeId}/{ModuleId}", EntityNames.Module, ModuleId));
        }
    }
}

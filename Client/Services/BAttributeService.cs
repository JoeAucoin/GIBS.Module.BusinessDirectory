using GIBS.Module.BusinessDirectory.Models;
using Oqtane.Enums;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GIBS.Module.BusinessDirectory.Services
{
    public class BAttributeService : ServiceBase, IBAttributeService, IService
    {
        public BAttributeService(HttpClient http, SiteState siteState) : base(http, siteState)
        {
        }

        private string Apiurl => CreateApiUrl("BAttribute");

        public async Task<IEnumerable<BAttribute>> GetAttributesAsync(int moduleId)
        {
            List<BAttribute> attributes = await GetJsonAsync<List<BAttribute>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={moduleId}", EntityNames.Module, moduleId), Enumerable.Empty<BAttribute>().ToList());
            return attributes.OrderBy(item => item.SortOrder).ThenBy(item => item.AttributeName);
        }

        //public async Task<IEnumerable<BAttribute>> GetAttributesAsync(int moduleId)
        //{
        //    List<BAttribute> attributes = await GetJsonAsync<List<BAttribute>>(
        //        CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={moduleId}", EntityNames.Module, moduleId),
        //        Enumerable.Empty<BAttribute>().ToList());

        //    return attributes.OrderBy(item => item.SortOrder).ThenBy(item => item.AttributeName);
        //}

        public async Task<BAttribute> GetAttributeAsync(int attributeId)
        {
            // Assuming moduleId is not required for this overload, using a default value or handling it differently
            return await GetJsonAsync<BAttribute>(CreateAuthorizationPolicyUrl($"{Apiurl}/{attributeId}", EntityNames.Module, 0));
        }

        public async Task<BAttribute> AddAttributeAsync(BAttribute battribute)
        {
            return await PostJsonAsync<BAttribute>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, battribute.ModuleId), battribute);
        }

        public async Task<BAttribute> UpdateAttributeAsync(BAttribute battribute)
        {
            return await PutJsonAsync<BAttribute>(CreateAuthorizationPolicyUrl($"{Apiurl}/{battribute.AttributeId}", EntityNames.Module, battribute.ModuleId), battribute);
        }

        public async Task DeleteAttributeAsync(int attributeId)
        {
            // Note: This implementation assumes the moduleId will be handled by the server
            // In a production scenario, you might want to modify this to include moduleId
            await DeleteAsync($"{Apiurl}/{attributeId}");
        }
    }
}

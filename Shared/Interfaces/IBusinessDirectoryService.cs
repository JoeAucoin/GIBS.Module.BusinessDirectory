using System.Collections.Generic;
using System.Threading.Tasks;

namespace GIBS.Module.BusinessDirectory.Services
{
    public interface IBusinessDirectoryService 
    {
        Task<List<Models.BusinessType>> GetBusinessDirectorysAsync(int ModuleId);

        Task<Models.BusinessType> GetBusinessDirectoryAsync(int TypeId, int ModuleId);

        Task<Models.BusinessType> AddBusinessDirectoryAsync(Models.BusinessType BusinessDirectory);

        Task<Models.BusinessType> UpdateBusinessDirectoryAsync(Models.BusinessType BusinessDirectory);

        Task DeleteBusinessDirectoryAsync(int TypeId, int ModuleId);
    }
}

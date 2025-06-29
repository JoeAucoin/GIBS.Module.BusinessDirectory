using System.Collections.Generic;
using System.Threading.Tasks;

namespace GIBS.Module.BusinessDirectory.Repository
{
    public interface IBusinessDirectoryRepository
    {
        IEnumerable<Models.BusinessType> GetBusinessDirectorys(int ModuleId);
        Models.BusinessType GetBusinessDirectory(int TypeId);
        Models.BusinessType GetBusinessDirectory(int TypeId, bool tracking);
        Models.BusinessType AddBusinessDirectory(Models.BusinessType BusinessDirectory);
        Models.BusinessType UpdateBusinessDirectory(Models.BusinessType BusinessDirectory);
        void DeleteBusinessDirectory(int TypeId);
    }
}

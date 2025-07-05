using System.Collections.Generic;
using System.Threading.Tasks;
using GIBS.Module.BusinessDirectory.Models;

namespace GIBS.Module.BusinessDirectory.Services
{
    public interface IBAttributeService
    {
        Task<IEnumerable<BAttribute>> GetAttributesAsync(int moduleId);
        Task<BAttribute> GetAttributeAsync(int attributeId);
        Task<BAttribute> AddAttributeAsync(BAttribute battribute);
        Task<BAttribute> UpdateAttributeAsync(BAttribute battribute);
        Task DeleteAttributeAsync(int attributeId);
    }
}

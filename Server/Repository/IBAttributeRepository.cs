using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GIBS.Module.BusinessDirectory.Models;

namespace GIBS.Module.BusinessDirectory.Repository
{
    public interface IBAttributeRepository
    {
        Task<IEnumerable<BAttribute>> GetAttributesAsync(int moduleId);
        Task<BAttribute> GetAttributeAsync(int attributeId);
        Task<BAttribute> AddAttributeAsync(BAttribute attribute);
        Task<BAttribute> UpdateAttributeAsync(BAttribute attribute);
        Task DeleteAttributeAsync(int attributeId);

    }
}

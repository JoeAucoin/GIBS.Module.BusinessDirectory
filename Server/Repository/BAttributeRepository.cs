using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GIBS.Module.BusinessDirectory.Models;

namespace GIBS.Module.BusinessDirectory.Repository
{
    public class BAttributeRepository : IBAttributeRepository
    {
        public readonly BusinessDirectoryContext _db;
        public readonly IDbContextFactory<BusinessDirectoryContext> _factory;

        public BAttributeRepository(BusinessDirectoryContext context, IDbContextFactory<BusinessDirectoryContext> factory)
        {
            _db = context;
            _factory = factory;
        }

        public async Task<IEnumerable<BAttribute>> GetAttributesAsync(int moduleId)
        {
            // Order by SortOrder, then by AttributeName for consistent ordering
            return await _db.BAttribute
                .Where(item => item.ModuleId == moduleId)
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.AttributeName)
                .AsNoTracking()
                .ToListAsync();
        }

        //public async Task<List<BAttribute>> GetAttributesAsync(int moduleId)
        //{
        //    using var context = _factory.CreateDbContext();
        //    return await context.BAttribute
        //        .Where(item => item.ModuleId == moduleId)
        //        .OrderBy(item => item.SortOrder)
        //        .ThenBy(item => item.AttributeName)
        //        .ToListAsync();
        //}

        public async Task<BAttribute> GetAttributeAsync(int attributeId)
        {
            // Use the correct DbSet for BAttribute
            return await _db.BAttribute.FindAsync(attributeId);
        }

        public async Task<BAttribute> AddAttributeAsync(BAttribute battribute)
        {
            _db.BAttribute.Add(battribute);
            await _db.SaveChangesAsync();
            return battribute;
        }

        public async Task<BAttribute> UpdateAttributeAsync(BAttribute battribute)
        {
            _db.BAttribute.Update(battribute);
            await _db.SaveChangesAsync();
            return battribute;
        }

        public async Task DeleteAttributeAsync(int attributeId)
        {
            var attribute = await _db.BAttribute.FindAsync(attributeId);
            if (attribute != null)
            {
                _db.BAttribute.Remove(attribute);
                await _db.SaveChangesAsync();
            }
        }

        //public async Task<int> GetMaxSortOrderForAttributeAsync(int moduleId)
        //{
        //    return await _db.BusinessAttribute
        //        .Where(a => a.ModuleId == moduleId)
        //        .Select(a => (int?)a.AttributeId)
        //        .DefaultIfEmpty(0)
        //        .MaxAsync();
        //}

        //public async Task<int> GetMaxSortOrderForAttributeAsync(int attributeId, int moduleId)
        //{
        //    return await _db.BusinessAttribute
        //        .Where(a => a.ModuleId == moduleId)
        //        .Select(a => (int?)a.SortOrder)
        //        .DefaultIfEmpty(0)
        //        .MaxAsync();
        //}
    }
}

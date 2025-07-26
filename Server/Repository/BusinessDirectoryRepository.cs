using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using GIBS.Module.BusinessDirectory.Models;
using System.Threading.Tasks;

namespace GIBS.Module.BusinessDirectory.Repository
{
    public class BusinessDirectoryRepository : IBusinessDirectoryRepository, ITransientService
    {
        private readonly IDbContextFactory<BusinessDirectoryContext> _factory;

        public BusinessDirectoryRepository(IDbContextFactory<BusinessDirectoryContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<BusinessType>> GetBusinessDirectorysAsync(int moduleId)
        {
            using var db = _factory.CreateDbContext();
            var types = await db.BusinessType
                .Where(item => item.ModuleId == moduleId)
                .OrderBy(item => item.SortOrder)
                .AsNoTracking()
                .ToListAsync();

            var companyCounts = await db.BusinessCompany
                .AsNoTracking()
                .Where(c => c.ModuleId == moduleId && c.IsActive)
                .GroupBy(c => c.TypeId)
                .Select(g => new { TypeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TypeId, x => x.Count);

            foreach (var type in types)
            {
                type.CompanyCount = companyCounts.GetValueOrDefault(type.TypeId, 0);
            }

            // New logic to aggregate counts up the hierarchy
            var typesById = types.ToDictionary(t => t.TypeId);
            foreach (var type in types.Where(t => t.ParentId != -1 && typesById.ContainsKey(t.ParentId)))
            {
                var parent = typesById[type.ParentId];
                while (parent != null)
                {
                    parent.CompanyCount += type.CompanyCount;
                    parent = parent.ParentId != -1 && typesById.ContainsKey(parent.ParentId) ? typesById[parent.ParentId] : null;
                }
            }

            return types;
        }

        public Models.BusinessType GetBusinessDirectoryAsync(int typeId, int moduleId)
        {
            using var db = _factory.CreateDbContext();
            return db.BusinessType
                .AsNoTracking()
                .FirstOrDefault(item => item.TypeId == typeId && item.ModuleId == moduleId);
        }

        public Models.BusinessType AddBusinessDirectoryAsync(Models.BusinessType businessDirectory)
        {
            using var db = _factory.CreateDbContext();
            db.BusinessType.Add(businessDirectory);
            db.SaveChanges();
            return businessDirectory;
        }

        public Models.BusinessType UpdateBusinessDirectoryAsync(Models.BusinessType businessDirectory)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(businessDirectory).State = EntityState.Modified;
            db.SaveChanges();
            return businessDirectory;
        }

        public void DeleteBusinessDirectoryAsync(int typeId, int moduleId)
        {
            using var db = _factory.CreateDbContext();
            var businessDirectory = db.BusinessType
                .FirstOrDefault(item => item.TypeId == typeId && item.ModuleId == moduleId);
            if (businessDirectory != null)
            {
                db.BusinessType.Remove(businessDirectory);
                db.SaveChanges();
            }
        }

        public IEnumerable<BusinessType> GetBusinessDirectorys(int ModuleId)
        {
            using var db = _factory.CreateDbContext();
            var types = db.BusinessType
                .Where(item => item.ModuleId == ModuleId)
                .OrderBy(item => item.SortOrder)
                .AsNoTracking()
                .ToList();

            var companyCounts = db.BusinessCompany
                .AsNoTracking()
                .Where(c => c.ModuleId == ModuleId && c.IsActive)
                .GroupBy(c => c.TypeId)
                .Select(g => new { TypeId = g.Key, Count = g.Count() })
                .ToDictionary(x => x.TypeId, x => x.Count);

            foreach (var type in types)
            {
                type.CompanyCount = companyCounts.GetValueOrDefault(type.TypeId, 0);
            }

            return types;
        }

        public BusinessType GetBusinessDirectory(int TypeId)
        {
            using var db = _factory.CreateDbContext();
            return db.BusinessType
                .AsNoTracking()
                .FirstOrDefault(item => item.TypeId == TypeId);
        }

        public BusinessType GetBusinessDirectory(int TypeId, bool tracking)
        {
            using var db = _factory.CreateDbContext();
            if (tracking)
            {
                return db.BusinessType.Find(TypeId);
            }
            else
            {
                return db.BusinessType.AsNoTracking().FirstOrDefault(item => item.TypeId == TypeId);
            }
        }

        public BusinessType AddBusinessDirectory(BusinessType BusinessDirectory)
        {
            using var db = _factory.CreateDbContext();
            db.BusinessType.Add(BusinessDirectory);
            db.SaveChanges();
            return BusinessDirectory;
        }

        public BusinessType UpdateBusinessDirectory(BusinessType BusinessDirectory)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(BusinessDirectory).State = EntityState.Modified;
            db.SaveChanges();
            return BusinessDirectory;
        }

        public void DeleteBusinessDirectory(int TypeId)
        {
            using var db = _factory.CreateDbContext();
            var businessDirectory = db.BusinessType.Find(TypeId);
            if (businessDirectory != null)
            {
                db.BusinessType.Remove(businessDirectory);
                db.SaveChanges();
            }
        }


    }
}
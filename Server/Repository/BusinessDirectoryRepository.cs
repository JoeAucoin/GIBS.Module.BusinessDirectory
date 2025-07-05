using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;
using GIBS.Module.BusinessDirectory.Models;

namespace GIBS.Module.BusinessDirectory.Repository
{
    public class BusinessDirectoryRepository : IBusinessDirectoryRepository, ITransientService
    {
        private readonly IDbContextFactory<BusinessDirectoryContext> _factory;

        public BusinessDirectoryRepository(IDbContextFactory<BusinessDirectoryContext> factory)
        {
            _factory = factory;
        }


        public IEnumerable<Models.BusinessType> GetBusinessDirectorysAsync(int moduleId)
        {
            using var db = _factory.CreateDbContext();
            return db.BusinessType
                .Where(item => item.ModuleId == moduleId)
                .OrderBy(item => item.SortOrder)
                .AsNoTracking()
                .ToList();
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
            return db.BusinessType
                .Where(item => item.ModuleId == ModuleId)
                .OrderBy(item => item.SortOrder)
                .AsNoTracking()
                .ToList();
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

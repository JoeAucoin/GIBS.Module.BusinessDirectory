using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using GIBS.Module.BusinessDirectory.Models;

namespace GIBS.Module.BusinessDirectory.Repository
{
    public class BusinessCompanyRepository : IBusinessCompanyRepository, ITransientService
    {
        private readonly IDbContextFactory<BusinessDirectoryContext> _factory;

        public BusinessCompanyRepository(IDbContextFactory<BusinessDirectoryContext> factory)
        {
            _factory = factory;
        }

        // Synchronous methods (optional, not in interface)
        public IEnumerable<BusinessCompany> GetBusinessCompanies(int moduleId)
        {
            using var db = _factory.CreateDbContext();
            return db.BusinessCompany
                .Where(item => item.ModuleId == moduleId)
                .OrderBy(item => item.SortOrder)
                .AsNoTracking()
                .ToList();
        }

        public BusinessCompany GetBusinessCompany(int companyId)
        {
            using var db = _factory.CreateDbContext();
            return db.BusinessCompany
                .Include(c => c.BusinessToAttribute)
                    .ThenInclude(bta => bta.BAttribute)
                .AsNoTracking()
                .FirstOrDefault(item => item.CompanyId == companyId);
        }

        public BusinessCompany AddBusinessCompany(BusinessCompany businessCompany)
        {
            using var db = _factory.CreateDbContext();
            db.BusinessCompany.Add(businessCompany);
            db.SaveChanges();
            return businessCompany;
        }

        public BusinessCompany UpdateBusinessCompany(BusinessCompany businessCompany)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(businessCompany).State = EntityState.Modified;
            db.SaveChanges();
            return businessCompany;
        }

        public void DeleteBusinessCompany(int companyId)
        {
            using var db = _factory.CreateDbContext();
            var businessCompany = db.BusinessCompany.Find(companyId);
            if (businessCompany != null)
            {
                db.BusinessCompany.Remove(businessCompany);
                db.SaveChanges();
            }
        }

        // Async interface methods
        //public async Task<BusinessCompany> GetBusinessCompanyAsync(int companyId, int moduleId)
        //{
        //    using var db = _factory.CreateDbContext();
        //    var company = await (
        //        from c in db.BusinessCompany
        //        join t in db.BusinessDirectory on c.TypeId equals t.TypeId into typeJoin
        //        from t in typeJoin.DefaultIfEmpty()
        //        where c.CompanyId == companyId && c.ModuleId == moduleId
        //        select new BusinessCompany
        //        {
        //            CompanyId = c.CompanyId,
        //            ModuleId = c.ModuleId,
        //            TypeId = c.TypeId,
        //            CompanyName = c.CompanyName,
        //            Address = c.Address,
        //            City = c.City,
        //            State = c.State,
        //            ZipCode = c.ZipCode,
        //            Phone = c.Phone,
        //            Email = c.Email,
        //            Website = c.Website,
        //            Description = c.Description,
        //            ImageURL = c.ImageURL,
        //            SortOrder = c.SortOrder,
        //            IsNewItem = c.IsNewItem,
        //            IsActive = c.IsActive,
        //            Latitude = c.Latitude,
        //            Longitude = c.Longitude,
        //            CreatedBy = c.CreatedBy,
        //            CreatedOn = c.CreatedOn,
        //            ModifiedBy = c.ModifiedBy,
        //            ModifiedOn = c.ModifiedOn,
        //            TypeName = t != null ? t.TypeName : null,
        //            TypeDescription = t != null ? t.TypeDescription : null,
        //            Slug = c.Slug // Include Slug if needed
        //        }
        //    ).AsNoTracking().FirstOrDefaultAsync();

        //    return company;
        //}

        public async Task<BusinessCompany> GetBusinessCompanyAsync(int companyId, int moduleId)
        {
            using var db = _factory.CreateDbContext();

            // Load company with attributes
            var company = await db.BusinessCompany
                .Include(c => c.BusinessToAttribute)
                    .ThenInclude(bta => bta.BAttribute)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.ModuleId == moduleId);

            if (company != null)
            {
                // Load type info for TypeName and TypeDescription
                var type = await db.BusinessType
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TypeId == company.TypeId && t.ModuleId == moduleId);

                company.TypeName = type?.TypeName;
                company.TypeDescription = type?.TypeDescription;
            }

            return company;
        }


        public async Task<BusinessCompany> AddBusinessCompanyAsync(BusinessCompany businessCompany)
        {
            using var db = _factory.CreateDbContext();
            db.BusinessCompany.Add(businessCompany);
            await db.SaveChangesAsync();
            return businessCompany;
        }

        public async Task<BusinessCompany> UpdateBusinessCompanyAsync(BusinessCompany businessCompany)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(businessCompany).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return businessCompany;
        }

        public async Task DeleteBusinessCompanyAsync(int companyId, int moduleId)
        {
            using var db = _factory.CreateDbContext();
            var businessCompany = await db.BusinessCompany
                .FirstOrDefaultAsync(item => item.CompanyId == companyId && item.ModuleId == moduleId);
            if (businessCompany != null)
            {
                db.BusinessCompany.Remove(businessCompany);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<BusinessCompany>> GetBusinessCompaniesAsync(int moduleId)
        {
            using var db = _factory.CreateDbContext();
            var companies = await (
                from c in db.BusinessCompany
                join t in db.BusinessType on c.TypeId equals t.TypeId into typeJoin
                from t in typeJoin.DefaultIfEmpty()
                where c.ModuleId == moduleId
                orderby c.SortOrder
                select new BusinessCompany
                {
                    CompanyId = c.CompanyId,
                    ModuleId = c.ModuleId,
                    TypeId = c.TypeId,
                    CompanyName = c.CompanyName,
                    Address = c.Address,
                    City = c.City,
                    State = c.State,
                    ZipCode = c.ZipCode,
                    Phone = c.Phone,
                    Email = c.Email,
                    Website = c.Website,
                    Description = c.Description,
                    ImageURL = c.ImageURL,
                    SortOrder = c.SortOrder,
                    IsNewItem = c.IsNewItem,
                    IsActive = c.IsActive,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    CreatedBy = c.CreatedBy,
                    CreatedOn = c.CreatedOn,
                    ModifiedBy = c.ModifiedBy,
                    ModifiedOn = c.ModifiedOn,
                    TypeName = t != null ? t.TypeName : null,
                    TypeDescription = t != null ? t.TypeDescription : null,
                    Slug = c.Slug // Include Slug if needed
                }
            ).AsNoTracking().ToListAsync();

            return companies;
        }

        public async Task<List<BusinessCompany>> GetBusinessCompaniesByTypeAsync(int typeId, int moduleId)
        {
            using var db = _factory.CreateDbContext();

            // Get all TypeIds: the requested one and all children
            var typeIds = await db.BusinessType
                .Where(t => t.ModuleId == moduleId && (t.TypeId == typeId || t.ParentId == typeId))
                .Select(t => t.TypeId)
                .ToListAsync();

            return await db.BusinessCompany
                .Where(c => c.ModuleId == moduleId && typeIds.Contains(c.TypeId))
                .OrderBy(c => c.SortOrder)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<BusinessCompany>> GetBusinessCompaniesByIsNewItemAsync(int moduleId)
        {
            using var db = _factory.CreateDbContext();
            var companies = await (
                from c in db.BusinessCompany
                join t in db.BusinessType on c.TypeId equals t.TypeId into typeJoin
                from t in typeJoin.DefaultIfEmpty()
                where c.ModuleId == moduleId && c.IsNewItem
                orderby c.SortOrder
                select new BusinessCompany
                {
                    CompanyId = c.CompanyId,
                    ModuleId = c.ModuleId,
                    TypeId = c.TypeId,
                    CompanyName = c.CompanyName,
                    Address = c.Address,
                    City = c.City,
                    State = c.State,
                    ZipCode = c.ZipCode,
                    Phone = c.Phone,
                    Email = c.Email,
                    Website = c.Website,
                    Description = c.Description,
                    ImageURL = c.ImageURL,
                    SortOrder = c.SortOrder,
                    IsNewItem = c.IsNewItem,
                    IsActive = c.IsActive,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude,
                    CreatedBy = c.CreatedBy,
                    CreatedOn = c.CreatedOn,
                    ModifiedBy = c.ModifiedBy,
                    ModifiedOn = c.ModifiedOn,
                    TypeName = t != null ? t.TypeName : null,
                    TypeDescription = t != null ? t.TypeDescription : null,
                    Slug = c.Slug // Include Slug if needed
                }
            ).AsNoTracking().ToListAsync();

            return companies;
        }

        public async Task UpdateCompanyAttributesAsync(int companyId, int moduleId, List<int> attributeIds)
        {
            using var db = _factory.CreateDbContext();

            // Remove existing attribute links for this company and module
            var existing = db.BusinessToAttribute
                .Where(x => x.CompanyId == companyId);
            db.BusinessToAttribute.RemoveRange(existing);

            // Add new attribute links
            if (attributeIds != null)
            {
                foreach (var attrId in attributeIds)
                {
                    db.BusinessToAttribute.Add(new BusinessToAttribute
                    {
                        CompanyId = companyId,
                        AttributeId = attrId
                    });
                }
            }

            await db.SaveChangesAsync();
        }

        public async Task<List<BusinessCompany>> GetCompanyAttributesAsync(int companyId, int moduleId)
        {
            using var db = _factory.CreateDbContext();

            try
            {
                // Get the company and load its BusinessToAttribute collection (with BAttribute)
                var company = await db.BusinessCompany
                    .Where(c => c.CompanyId == companyId && c.ModuleId == moduleId)
                    .Include(c => c.BusinessToAttribute)
                        .ThenInclude(bta => bta.BAttribute)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                // DEBUG: Log the count of attributes loaded from the DB
                var attrCount = company?.BusinessToAttribute?.Count ?? 0;
             //   Console.WriteLine($"[DEBUG] EF loaded {attrCount} attributes for company {companyId}");

                // Direct query for debugging
                var directAttrs = db.BusinessToAttribute.Where(x => x.CompanyId == companyId).ToList();
             //   Console.WriteLine($"[DEBUG] Direct query found {directAttrs.Count} BusinessToAttribute records for company {companyId}");

                if (company == null)
                {
                    return new List<BusinessCompany>();
                }

                company.BusinessToAttribute = company.BusinessToAttribute?.ToList() ?? new List<BusinessToAttribute>();

                return new List<BusinessCompany> { company };
            }
            catch (Exception ex)
            {
                // Log the error and return empty list
            //    Console.WriteLine($"Error in GetCompanyAttributesAsync: {ex.Message}");
                return new List<BusinessCompany>();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Oqtane.Modules;
using Oqtane.Models;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Enums;
using Oqtane.Repository;
using GIBS.Module.BusinessDirectory.Repository;
using System.Threading.Tasks;

namespace GIBS.Module.BusinessDirectory.Manager
{
    public class BusinessDirectoryManager : MigratableModuleBase, IInstallable, IPortable, ISearchable
    {
        private readonly IBusinessDirectoryRepository _BusinessDirectoryRepository;
        private readonly IDBContextDependencies _DBContextDependencies;

        public BusinessDirectoryManager(IBusinessDirectoryRepository BusinessDirectoryRepository, IDBContextDependencies DBContextDependencies)
        {
            _BusinessDirectoryRepository = BusinessDirectoryRepository;
            _DBContextDependencies = DBContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new BusinessDirectoryContext(_DBContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new BusinessDirectoryContext(_DBContextDependencies), tenant, MigrationType.Down);
        }

        public string ExportModule(Oqtane.Models.Module module)
        {
            string content = "";

            // Export all relevant entities
            var exportDto = new Models.BusinessDirectoryExportDto
            {
                BusinessTypes = _BusinessDirectoryRepository.GetBusinessDirectorysAsync(module.ModuleId).GetAwaiter().GetResult()
            };

            // You may need to inject and use the appropriate repositories/services for these:
            using (var db = new Repository.BusinessDirectoryContext(_DBContextDependencies))
            {
                exportDto.BusinessCompanies = db.BusinessCompany.Where(x => x.ModuleId == module.ModuleId).ToList();
                exportDto.BAttributes = db.BAttribute.Where(x => x.ModuleId == module.ModuleId).ToList();
                exportDto.BusinessToAttributes = db.BusinessToAttribute
                    .Where(x => exportDto.BusinessCompanies.Select(c => c.CompanyId).Contains(x.CompanyId))
                    .ToList();
            }

            content = JsonSerializer.Serialize(exportDto);
            return content;
        }

        public void ImportModule(Oqtane.Models.Module module, string content, string version)
        {
            if (string.IsNullOrEmpty(content)) return;

            var exportDto = JsonSerializer.Deserialize<Models.BusinessDirectoryExportDto>(content);
            if (exportDto == null) return;

            // Import order: Types -> Attributes -> Companies -> BusinessToAttributes
            foreach (var type in exportDto.BusinessTypes)
            {
                type.ModuleId = module.ModuleId;
                _BusinessDirectoryRepository.AddBusinessDirectory(type);
            }

            using (var db = new Repository.BusinessDirectoryContext(_DBContextDependencies))
            {
                foreach (var attr in exportDto.BAttributes)
                {
                    attr.ModuleId = module.ModuleId;
                    db.BAttribute.Add(attr);
                }
                db.SaveChanges();

                foreach (var company in exportDto.BusinessCompanies)
                {
                    company.ModuleId = module.ModuleId;
                    db.BusinessCompany.Add(company);
                }
                db.SaveChanges();

                foreach (var bta in exportDto.BusinessToAttributes)
                {
                    db.BusinessToAttribute.Add(bta);
                }
                db.SaveChanges();
            }
        }

        public async Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
        {
            var searchContentList = new List<SearchContent>();

            foreach (var BusinessDirectory in await _BusinessDirectoryRepository.GetBusinessDirectorysAsync(pageModule.ModuleId))
            {
                if (BusinessDirectory.ModifiedOn >= lastIndexedOn)
                {
                    searchContentList.Add(new SearchContent
                    {
                        EntityName = "GIBSBusinessType",
                        EntityId = BusinessDirectory.TypeId.ToString(),
                        Title = BusinessDirectory.TypeName,
                        Body = BusinessDirectory.TypeDescription,
                        ContentModifiedBy = BusinessDirectory.ModifiedBy,
                        ContentModifiedOn = BusinessDirectory.ModifiedOn
                    });
                }
            }

            return searchContentList;
        }
    }
}
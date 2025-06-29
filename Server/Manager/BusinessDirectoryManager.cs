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
            List<Models.BusinessType> BusinessDirectorys = _BusinessDirectoryRepository.GetBusinessDirectorys(module.ModuleId).ToList();
            if (BusinessDirectorys != null)
            {
                content = JsonSerializer.Serialize(BusinessDirectorys);
            }
            return content;
        }

        public void ImportModule(Oqtane.Models.Module module, string content, string version)
        {
            List<Models.BusinessType> BusinessDirectorys = null;
            if (!string.IsNullOrEmpty(content))
            {
                BusinessDirectorys = JsonSerializer.Deserialize<List<Models.BusinessType>>(content);
            }
            if (BusinessDirectorys != null)
            {
                foreach(var BusinessDirectory in BusinessDirectorys)
                {
                    _BusinessDirectoryRepository.AddBusinessDirectory(new Models.BusinessType { ModuleId = module.ModuleId, TypeName = BusinessDirectory.TypeName });
                }
            }
        }

        public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
        {
           var searchContentList = new List<SearchContent>();

           foreach (var BusinessDirectory in _BusinessDirectoryRepository.GetBusinessDirectorys(pageModule.ModuleId))
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

           return Task.FromResult(searchContentList);
        }
    }
}

using Oqtane.Models;
using Oqtane.Modules;

namespace GIBS.Module.BusinessDirectory
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "BusinessDirectory",
            Description = "Business Directory module for Oqtane",
            Version = "1.0.4",
            ServerManagerType = "GIBS.Module.BusinessDirectory.Manager.BusinessDirectoryManager, GIBS.Module.BusinessDirectory.Server.Oqtane",
            ReleaseVersions = "1.0.0,1.0.1,1.0.3,1.0.4",
            Dependencies = "GIBS.Module.BusinessDirectory.Shared.Oqtane",
            PackageName = "GIBS.Module.BusinessDirectory" 
        };
    }
}

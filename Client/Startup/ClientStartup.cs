using GIBS.Module.BusinessDirectory.Services;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Services;

namespace GIBS.Module.BusinessDirectory.Startup
{
    public class ClientStartup : IClientStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IBusinessDirectoryService, BusinessDirectoryService>();
            services.AddScoped<IBusinessCompanyService, BusinessCompanyService>();

            // Register BusinessCompanyService with HttpClient using AddHttpClient
          //  services.AddHttpClient<IBusinessCompanyService, BusinessCompanyService>();
        }
    }
}

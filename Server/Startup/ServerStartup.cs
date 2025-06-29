using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using GIBS.Module.BusinessDirectory.Repository;
using GIBS.Module.BusinessDirectory.Services;

namespace GIBS.Module.BusinessDirectory.Startup
{
    public class ServerStartup : IServerStartup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // not implemented
        }

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            // not implemented
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IBusinessDirectoryService, ServerBusinessDirectoryService>();
            services.AddTransient<IBusinessDirectoryRepository, BusinessDirectoryRepository>();



            services.AddDbContextFactory<BusinessDirectoryContext>(opt => { }, ServiceLifetime.Transient);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Infrastructure;
using Oqtane.Repository.Databases.Interfaces;
using System.Collections.Generic;

namespace GIBS.Module.BusinessDirectory.Repository
{
    public class BusinessDirectoryContext : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<Models.BusinessType> BusinessDirectory { get; set; }
      //  public IEnumerable<object> BusinessCompany { get; internal set; }

        public virtual DbSet<Models.BusinessCompany> BusinessCompany { get; set; }

        public BusinessDirectoryContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
        {
            // ContextBase handles multi-tenant database connections
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Models.BusinessType>().ToTable(ActiveDatabase.RewriteName("GIBSBusinessType"));
        }


    }
}

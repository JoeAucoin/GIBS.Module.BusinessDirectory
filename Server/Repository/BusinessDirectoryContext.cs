using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using Oqtane.Repository;
using GIBS.Module.BusinessDirectory.Models;
using Oqtane.Repository.Databases.Interfaces;

namespace GIBS.Module.BusinessDirectory.Repository
{
    public class BusinessDirectoryContext : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<BusinessType> BusinessType { get; set; }
        public virtual DbSet<BusinessCompany> BusinessCompany { get; set; }
        public virtual DbSet<BusinessToAttribute> BusinessToAttribute { get; set; }
        public virtual DbSet<BAttribute> BAttribute { get; set; }

        public BusinessDirectoryContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
        {
            // ContextBase handles multi-tenant database connections
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BusinessType>().ToTable(ActiveDatabase.RewriteName("GIBSBusinessType"));
            builder.Entity<BusinessCompany>().ToTable(ActiveDatabase.RewriteName("GIBSBusinessCompany"));
            builder.Entity<BAttribute>().ToTable(ActiveDatabase.RewriteName("GIBSBusinessAttribute"));
            builder.Entity<BusinessToAttribute>().ToTable(ActiveDatabase.RewriteName("GIBSBusinessToAttribute"));

            // Configure BusinessToAttribute relationships
            builder.Entity<BusinessToAttribute>()
                .HasOne(bta => bta.BusinessCompany)
                .WithMany(bc => bc.BusinessToAttribute)
                .HasForeignKey(bta => bta.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BusinessToAttribute>()
                .HasOne(bta => bta.BAttribute)
                .WithMany()
                .HasForeignKey(bta => bta.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

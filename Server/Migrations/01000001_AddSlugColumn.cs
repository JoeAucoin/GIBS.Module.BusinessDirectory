using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using GIBS.Module.BusinessDirectory.Migrations.EntityBuilders;
using GIBS.Module.BusinessDirectory.Repository;

namespace GIBS.Module.BusinessDirectory.Server.Migrations
{
    [DbContext(typeof(BusinessDirectoryContext))]
    [Migration("BusinessDirectory.01.00.00.01")]
    public class _01000001_AddSlugColumn : MultiDatabaseMigration
    {
        public _01000001_AddSlugColumn(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This migration is now redundant since Slug columns are created in the initial migration
            // Keeping this as a placeholder for version compatibility
            // No operations needed - Slug columns and indexes are already created in 01000000_InitializeModule
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No operations needed - rollback handled in the initial migration
        }
    }
}

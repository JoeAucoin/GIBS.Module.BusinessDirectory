using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using GIBS.Module.BusinessDirectory.Migrations.EntityBuilders;
using GIBS.Module.BusinessDirectory.Repository;

namespace GIBS.Module.BusinessDirectory.Migrations
{
    [DbContext(typeof(BusinessDirectoryContext))]
    [Migration("GIBS.Module.BusinessDirectory.01.00.00.00")]
    public class InitializeModule : MultiDatabaseMigration
    {
        public InitializeModule(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new BusinessDirectoryEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Create();

            var businessEntityBuilder = new BusinessEntityBuilder(migrationBuilder, ActiveDatabase);
            businessEntityBuilder.Create();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new BusinessDirectoryEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Drop();

            var businessEntityBuilder = new BusinessEntityBuilder(migrationBuilder, ActiveDatabase);
            businessEntityBuilder.Drop();
        }
    }
}

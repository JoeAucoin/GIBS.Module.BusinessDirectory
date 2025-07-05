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
            // Create BusinessType table first (no dependencies)
            var entityBuilder = new BusinessDirectoryEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Create();

            // Create BusinessCompany table second (depends on BusinessType)
            var businessEntityBuilder = new BusinessEntityBuilder(migrationBuilder, ActiveDatabase);
            businessEntityBuilder.Create();

            // Add indexes for Slug columns
            migrationBuilder.CreateIndex(
                name: "IX_GIBSBusinessType_Slug",
                table: ActiveDatabase.RewriteName("GIBSBusinessType"),
                column: "Slug",
                unique: false);

            migrationBuilder.CreateIndex(
                name: "IX_GIBSBusinessCompany_Slug", 
                table: ActiveDatabase.RewriteName("GIBSBusinessCompany"),
                column: "Slug",
                unique: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes first
            migrationBuilder.DropIndex(
                name: "IX_GIBSBusinessCompany_Slug",
                table: ActiveDatabase.RewriteName("GIBSBusinessCompany"));

            migrationBuilder.DropIndex(
                name: "IX_GIBSBusinessType_Slug", 
                table: ActiveDatabase.RewriteName("GIBSBusinessType"));

            // Drop tables in reverse order
            var businessEntityBuilder = new BusinessEntityBuilder(migrationBuilder, ActiveDatabase);
            businessEntityBuilder.Drop();

            var entityBuilder = new BusinessDirectoryEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Drop();
        }
    }
}

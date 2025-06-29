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
            var BusinessEntityBuilder = new BusinessEntityBuilder(migrationBuilder, ActiveDatabase);
            BusinessEntityBuilder.AddStringColumn("Slug", 500, true);
            BusinessEntityBuilder.AddIndex("IX_BusinessCompany_Slug", "Slug", true);

            var BusinessDirectoryBuilder = new BusinessDirectoryEntityBuilder(migrationBuilder, ActiveDatabase);
            BusinessDirectoryBuilder.AddStringColumn("Slug", 500, true);
            BusinessDirectoryBuilder.AddIndex("IX_BusinessType_Slug", "Slug", true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //var BusinessEntityBuilder = new BusinessEntityBuilder(migrationBuilder, ActiveDatabase);
            //BusinessEntityBuilder.DropIndex("IX_BusinessCompany_Slug", "Slug");
            //BusinessEntityBuilder.DropColumn("Slug", "GIBSBusinessCompany"); // Removed the second argument to match the method signature

            //var BusinessDirectoryBuilder = new BusinessDirectoryEntityBuilder(migrationBuilder, ActiveDatabase);
            //BusinessDirectoryBuilder.DropIndex("IX_BusinessType_Slug", "Slug");
            //BusinessDirectoryBuilder.DropColumn("Slug", "GIBSBusinessType"); // Removed the second argument to match the method signature
        }
    }
}

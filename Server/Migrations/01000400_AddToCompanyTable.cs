using GIBS.Module.BusinessDirectory.Migrations.EntityBuilders;
using GIBS.Module.BusinessDirectory.Repository;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;


namespace GIBS.Module.BusinessDirectory.Migrations
{
    [DbContext(typeof(BusinessDirectoryContext))]
    [Migration("GIBS.Module.BusinessDirectory.01.00.04.00")]
   
    public class AddToCompanyTable : MultiDatabaseMigration
    {
        public AddToCompanyTable(IDatabase database) : base(database)
        {
        }
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var businessEntityBuilder = new BusinessEntityBuilder(migrationBuilder, ActiveDatabase);
            businessEntityBuilder.AddMaxStringColumn("HtmlContent", true, true);
            businessEntityBuilder.AddMaxStringColumn("EmbedVideo", true, true);
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var businessEntityBuilder = new BusinessEntityBuilder(migrationBuilder, ActiveDatabase);
            businessEntityBuilder.DropColumn("HtmlContent");
            businessEntityBuilder.DropColumn("EmbedVideo");
        }
    }
}

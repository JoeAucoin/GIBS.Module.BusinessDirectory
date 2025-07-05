using GIBS.Module.BusinessDirectory.Migrations.EntityBuilders;
using GIBS.Module.BusinessDirectory.Repository;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;


namespace GIBS.Module.BusinessDirectory.Migrations
{
    [DbContext(typeof(BusinessDirectoryContext))]
    [Migration("GIBS.Module.BusinessDirectory.01.00.03.00")]
    public class AddAttributeTable : MultiDatabaseMigration
    {
        public AddAttributeTable(IDatabase database) : base(database)
        {
        }
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var attributeEntityBuilder = new AttributeEntityBuilder(migrationBuilder, ActiveDatabase);
            // Create the GIBSBusinessAttribute table using the entity builder
            attributeEntityBuilder.Create();

            var entityBuilder = new BusinessAttributeEntityBuilder(migrationBuilder, ActiveDatabase);
            // Create the GIBSBusinessAttribute table using the entity builder
            entityBuilder.Create();

        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var attributeEntityBuilder = new AttributeEntityBuilder(migrationBuilder, ActiveDatabase);
            // Drop the GIBSBusinessAttribute table using the entity builder
            attributeEntityBuilder.Drop();

            var entityBuilder = new BusinessAttributeEntityBuilder(migrationBuilder, ActiveDatabase);
            // Drop the GIBSBusinessAttribute table using the entity builder
            entityBuilder.Drop();
        }
    }
}

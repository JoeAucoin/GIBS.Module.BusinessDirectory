using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace GIBS.Module.BusinessDirectory.Migrations.EntityBuilders
{
    public class BusinessAttributeEntityBuilder : AuditableBaseEntityBuilder<BusinessAttributeEntityBuilder>
    {
        private const string _entityTableName = "GIBSBusinessToAttribute";
        private readonly PrimaryKey<BusinessAttributeEntityBuilder> _primaryKey = new("PK_GIBSBusinessToAttribute", x => x.BusinessAttributeId);
        private readonly ForeignKey<BusinessAttributeEntityBuilder> _businessForeignKey = new("FK_GIBSAttribute_Business", x => x.CompanyId, "GIBSBusinessCompany", "CompanyId", ReferentialAction.Cascade);
        public BusinessAttributeEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_businessForeignKey);
        }
        protected override BusinessAttributeEntityBuilder BuildTable(ColumnsBuilder table)
        {
            BusinessAttributeId = AddAutoIncrementColumn(table, "BusinessAttributeId");
            CompanyId = AddIntegerColumn(table, "CompanyId");
            AttributeId = AddIntegerColumn(table, "AttributeId");

            AddAuditableColumns(table);
            return this;
        }
        public OperationBuilder<AddColumnOperation> BusinessAttributeId { get; set; }
        public OperationBuilder<AddColumnOperation> CompanyId { get; set; }
        public OperationBuilder<AddColumnOperation> AttributeId { get; set; }

    }
}

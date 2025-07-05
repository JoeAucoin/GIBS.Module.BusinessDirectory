using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace GIBS.Module.BusinessDirectory.Migrations.EntityBuilders
{
    public class AttributeEntityBuilder : AuditableBaseEntityBuilder<AttributeEntityBuilder>
    {
        private const string _entityTableName = "GIBSBusinessAttribute";
        private readonly PrimaryKey<AttributeEntityBuilder> _primaryKey = new("PK_GIBSBusinessAttribute", x => x.AttributeId);
        private readonly ForeignKey<AttributeEntityBuilder> _moduleForeignKey = new("FK_GIBSBusinessAttribute_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public AttributeEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }
        protected override AttributeEntityBuilder BuildTable(ColumnsBuilder table)
        {
            AttributeId = AddAutoIncrementColumn(table, "AttributeId");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            AttributeName = AddStringColumn(table, "AttributeName",250,false);
            AttributeDescription = table.Column<string>(name: "AttributeDescription", maxLength: int.MaxValue, nullable: true);
            AttributeIcon = table.Column<string>(name: "AttributeIcon", maxLength: int.MaxValue, nullable: true);
            AttributeCode = AddStringColumn(table, "AttributeCode", 2);
            AttributeColor = AddStringColumn(table, "AttributeColor", 7);
            SortOrder = table.Column<int>(name: "SortOrder", nullable: false, defaultValue: 0);
            IsActive = table.Column<bool>(name: "IsActive", nullable: false, defaultValue: true);
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> AttributeId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> AttributeName { get; set; }
        public OperationBuilder<AddColumnOperation> AttributeDescription { get; set; }
        public OperationBuilder<AddColumnOperation> AttributeIcon { get; set; }
        public OperationBuilder<AddColumnOperation> AttributeCode { get; set; }
        public OperationBuilder<AddColumnOperation> AttributeColor { get; set; }
        public OperationBuilder<AddColumnOperation> SortOrder { get; set; }
        public OperationBuilder<AddColumnOperation> IsActive { get; set; }
    }
}

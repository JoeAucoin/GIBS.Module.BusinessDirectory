using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;
using System;

namespace GIBS.Module.BusinessDirectory.Migrations.EntityBuilders
{
    public class BusinessDirectoryEntityBuilder : AuditableBaseEntityBuilder<BusinessDirectoryEntityBuilder>
    {
        private const string _entityTableName = "GIBSBusinessType";
        private readonly PrimaryKey<BusinessDirectoryEntityBuilder> _primaryKey = new("PK_GIBSBusinessType", x => x.TypeId);
        private readonly ForeignKey<BusinessDirectoryEntityBuilder> _moduleForeignKey = new("FK_GIBSBusinessType_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public BusinessDirectoryEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override BusinessDirectoryEntityBuilder BuildTable(ColumnsBuilder table)
        {
            TypeId = AddAutoIncrementColumn(table,"TypeId");
            ModuleId = AddIntegerColumn(table,"ModuleId");
            TypeName = table.Column<string>(name: "TypeName", maxLength: 256, nullable: true);
            TypeDescription = table.Column<string>(name: "TypeDescription", maxLength: int.MaxValue, nullable: true);
            ImageURL = table.Column<string>(name: "ImageURL", maxLength: int.MaxValue, nullable: true);         
            SortOrder = table.Column<int>(name: "SortOrder", nullable: false, defaultValue: 0);
            IsNewItem = table.Column<bool>(name: "IsNewItem", nullable: false, defaultValue: false);
            IsActive = table.Column<bool>(name: "IsActive", nullable: false, defaultValue: true);
            ParentId = table.Column<int>(name: "ParentId", nullable: false, defaultValue: -1);   
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> TypeId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> TypeName { get; set; }
        public OperationBuilder<AddColumnOperation> TypeDescription { get; set; }
        public OperationBuilder<AddColumnOperation> ImageURL { get; set; }
        public OperationBuilder<AddColumnOperation> SortOrder { get; set; }
        public OperationBuilder<AddColumnOperation> IsNewItem { get; set; }
        public OperationBuilder<AddColumnOperation> IsActive { get; set; }
        public OperationBuilder<AddColumnOperation> ParentId { get; set; }
    }
}

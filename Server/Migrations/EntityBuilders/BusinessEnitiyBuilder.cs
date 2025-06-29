using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;
using System;

namespace GIBS.Module.BusinessDirectory.Migrations.EntityBuilders
{
    public class BusinessEntityBuilder : AuditableBaseEntityBuilder<BusinessEntityBuilder>
    {

        private const string _entityTableName = "GIBSBusinessCompany";
        private readonly PrimaryKey<BusinessEntityBuilder> _primaryKey = new("PK_GIBSBusinessCompany", x => x.CompanyId);
        private readonly ForeignKey<BusinessEntityBuilder> _moduleForeignKey = new("FK_GIBSBusiness_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);
        private readonly ForeignKey<BusinessEntityBuilder> _businessTypeForeignKey = new("FK_GIBSBusiness_Type", x => x.TypeId, "GIBSBusinessDirectoryType", "TypeId", ReferentialAction.NoAction);
        public BusinessEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }
        protected override BusinessEntityBuilder BuildTable(ColumnsBuilder table)
        {
            CompanyId = AddAutoIncrementColumn(table, "CompanyId");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            TypeId = AddIntegerColumn(table, "TypeId");
            CompanyName = table.Column<string>(name: "CompanyName", maxLength: 256, nullable: false);
            Address = table.Column<string>(name: "Address", maxLength: 150, nullable: true);
            City = table.Column<string>(name: "City", maxLength: 100, nullable: true);
            State = table.Column<string>(name: "State", maxLength: 2, nullable: true);
            ZipCode = table.Column<string>(name: "ZipCode", maxLength: 10, nullable: true);
            Phone = table.Column<string>(name: "Phone", maxLength: 50, nullable: true);
            Email = table.Column<string>(name: "Email", maxLength: 100, nullable: true);
            Website = table.Column<string>(name: "Website", maxLength: 256, nullable: true);
            Description = table.Column<string>(name: "Description", maxLength: int.MaxValue, nullable: true);
            ImageURL = table.Column<string>(name: "ImageURL", maxLength: int.MaxValue, nullable: true);
            SortOrder = table.Column<int>(name: "SortOrder", nullable: false, defaultValue: 0);
            IsNewItem = table.Column<bool>(name: "IsNewItem", nullable: false, defaultValue: false);
            IsActive = table.Column<bool>(name: "IsActive", nullable: false, defaultValue: true);
            Latitude = table.Column<double>(name: "Latitude", nullable: false, defaultValue: 0.0);
            Longitude = table.Column<double>(name: "Longitude", nullable: false, defaultValue: 0.0);

            AddAuditableColumns(table);
            return this;
        }
        public OperationBuilder<AddColumnOperation> CompanyId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> TypeId { get; set; }
        public OperationBuilder<AddColumnOperation> CompanyName { get; set; }
        public OperationBuilder<AddColumnOperation> Address { get; set; } 
        public OperationBuilder<AddColumnOperation> City { get; set; }
        public OperationBuilder<AddColumnOperation> State { get; set; }
        public OperationBuilder<AddColumnOperation> ZipCode { get; set; }
        public OperationBuilder<AddColumnOperation> Phone { get; set; }
        public OperationBuilder<AddColumnOperation> Email { get; set; }
        public OperationBuilder<AddColumnOperation> Website { get; set; }
        public OperationBuilder<AddColumnOperation> Description { get; set; }
        public OperationBuilder<AddColumnOperation> ImageURL { get; set; }
        public OperationBuilder<AddColumnOperation> SortOrder { get; set; }
        public OperationBuilder<AddColumnOperation> IsNewItem { get; set; }
        public OperationBuilder<AddColumnOperation> IsActive { get; set; }
        public OperationBuilder<AddColumnOperation> Latitude { get; set; } 
        public OperationBuilder<AddColumnOperation> Longitude { get; set; }
    }
}

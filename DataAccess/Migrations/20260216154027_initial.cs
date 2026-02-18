using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AddedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastLoginUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypeFieldDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsSearchable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsSortable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypeFieldDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTypeFieldDefinitions_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PermissionDependencies",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    DependsOnPermissionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionDependencies", x => new { x.PermissionId, x.DependsOnPermissionId });
                    table.CheckConstraint("CK_PermissionDependencies_NoSelfDependency", "[PermissionId] <> [DependsOnPermissionId]");
                    table.ForeignKey(
                        name: "FK_PermissionDependencies_Permissions_DependsOnPermissionId",
                        column: x => x.DependsOnPermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermissionDependencies_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DocumentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupPermissions",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    DocumentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AddedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    AddedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPermissions", x => new { x.GroupId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_GroupPermissions_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupPermissions_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPermissions_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UserGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroups_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissionGrants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    DocumentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GrantedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissionGrants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissionGrants_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserPermissionGrants_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissionGrants_Users_GrantedByUserId",
                        column: x => x.GrantedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserPermissionGrants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypeFieldOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypeFieldOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTypeFieldOptions_DocumentTypeFieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "DocumentTypeFieldDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    ChangeNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentVersions_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentVersions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypeFieldValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false),
                    ValueString = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ValueInt = table.Column<int>(type: "int", nullable: true),
                    ValueDecimal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ValueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValueOptionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypeFieldValues", x => x.Id);
                    table.CheckConstraint("CK_DocumentTypeFieldValues_ExactlyOneValue", "(\r\n                        (CASE WHEN [ValueString]   IS NULL THEN 0 ELSE 1 END) +\r\n                        (CASE WHEN [ValueInt]      IS NULL THEN 0 ELSE 1 END) +\r\n                        (CASE WHEN [ValueDecimal]  IS NULL THEN 0 ELSE 1 END) +\r\n                        (CASE WHEN [ValueDate]     IS NULL THEN 0 ELSE 1 END) +\r\n                        (CASE WHEN [ValueOptionId] IS NULL THEN 0 ELSE 1 END)\r\n                      ) = 1");
                    table.ForeignKey(
                        name: "FK_DocumentTypeFieldValues_DocumentTypeFieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "DocumentTypeFieldDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentTypeFieldValues_DocumentTypeFieldOptions_ValueOptionId",
                        column: x => x.ValueOptionId,
                        principalTable: "DocumentTypeFieldOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentTypeFieldValues_DocumentVersions_VersionId",
                        column: x => x.VersionId,
                        principalTable: "DocumentVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatedBy",
                table: "Documents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_IsDeleted",
                table: "Documents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeFieldDefinitions_DocumentTypeId_Code",
                table: "DocumentTypeFieldDefinitions",
                columns: new[] { "DocumentTypeId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeFieldDefinitions_IsDeleted",
                table: "DocumentTypeFieldDefinitions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeFieldOptions_FieldDefinitionId_Value",
                table: "DocumentTypeFieldOptions",
                columns: new[] { "FieldDefinitionId", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeFieldOptions_IsDeleted",
                table: "DocumentTypeFieldOptions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeFieldValues_FieldDefinitionId",
                table: "DocumentTypeFieldValues",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeFieldValues_ValueOptionId",
                table: "DocumentTypeFieldValues",
                column: "ValueOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeFieldValues_VersionId_FieldDefinitionId",
                table: "DocumentTypeFieldValues",
                columns: new[] { "VersionId", "FieldDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_IsDeleted",
                table: "DocumentTypes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_Name",
                table: "DocumentTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_CreatedBy",
                table: "DocumentVersions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_DocumentId_VersionNumber",
                table: "DocumentVersions",
                columns: new[] { "DocumentId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_IsDeleted",
                table: "DocumentVersions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissions_AddedByUserId",
                table: "GroupPermissions",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissions_DocumentTypeId",
                table: "GroupPermissions",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissions_PermissionId",
                table: "GroupPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_IsActive",
                table: "Groups",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_IsDeleted",
                table: "Groups",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionDependencies_DependsOnPermissionId",
                table: "PermissionDependencies",
                column: "DependsOnPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionDependencies_PermissionId",
                table: "PermissionDependencies",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_IsActive",
                table: "Permissions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionGrants_DocumentTypeId",
                table: "UserPermissionGrants",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionGrants_GrantedByUserId",
                table: "UserPermissionGrants",
                column: "GrantedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionGrants_PermissionId",
                table: "UserPermissionGrants",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissionGrants_UserId",
                table: "UserPermissionGrants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted",
                table: "Users",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentTypeFieldValues");

            migrationBuilder.DropTable(
                name: "GroupPermissions");

            migrationBuilder.DropTable(
                name: "PermissionDependencies");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "UserPermissionGrants");

            migrationBuilder.DropTable(
                name: "DocumentTypeFieldOptions");

            migrationBuilder.DropTable(
                name: "DocumentVersions");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "DocumentTypeFieldDefinitions");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

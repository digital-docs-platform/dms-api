using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Users_UserId1",
                table: "UserGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermission_Users_UserId1",
                table: "UserPermission");

            migrationBuilder.DropIndex(
                name: "IX_UserPermission_UserId1",
                table: "UserPermission");

            migrationBuilder.DropIndex(
                name: "IX_UserGroups_UserId1",
                table: "UserGroups");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserPermission");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserGroups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "UserPermission",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "UserGroups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermission_UserId1",
                table: "UserPermission",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserId1",
                table: "UserGroups",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Users_UserId1",
                table: "UserGroups",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermission_Users_UserId1",
                table: "UserPermission",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

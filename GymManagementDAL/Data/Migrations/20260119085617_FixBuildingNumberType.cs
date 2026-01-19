using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementDAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBuildingNumberType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BuldingNumber",
                table: "Trainers",
                newName: "BuildingNumber");

            migrationBuilder.RenameColumn(
                name: "BuldingNumber",
                table: "Members",
                newName: "BuildingNumber");

            migrationBuilder.AlterColumn<int>(
                name: "BuildingNumber",
                table: "Trainers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "BuildingNumber",
                table: "Members",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BuildingNumber",
                table: "Trainers",
                newName: "BuldingNumber");

            migrationBuilder.RenameColumn(
                name: "BuildingNumber",
                table: "Members",
                newName: "BuldingNumber");

            migrationBuilder.AlterColumn<string>(
                name: "BuldingNumber",
                table: "Trainers",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "BuldingNumber",
                table: "Members",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}

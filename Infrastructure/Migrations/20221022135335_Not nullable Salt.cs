using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class NotnullableSalt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WebAdress",
                table: "SavedPasswords",
                newName: "WebAddress");

            migrationBuilder.AlterColumn<string>(
                name: "Salt",
                table: "Accounts",
                type: "TEXT",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 512,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WebAddress",
                table: "SavedPasswords",
                newName: "WebAdress");

            migrationBuilder.AlterColumn<string>(
                name: "Salt",
                table: "Accounts",
                type: "TEXT",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 512);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifySharedPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedPassword_Accounts_AccountId",
                table: "SharedPassword");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedPassword_SavedPasswords_SavedPasswordId",
                table: "SharedPassword");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SharedPassword",
                table: "SharedPassword");

            migrationBuilder.RenameTable(
                name: "SharedPassword",
                newName: "SharedPasswords");

            migrationBuilder.RenameIndex(
                name: "IX_SharedPassword_SavedPasswordId",
                table: "SharedPasswords",
                newName: "IX_SharedPasswords_SavedPasswordId");

            migrationBuilder.RenameIndex(
                name: "IX_SharedPassword_AccountId",
                table: "SharedPasswords",
                newName: "IX_SharedPasswords_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharedPasswords",
                table: "SharedPasswords",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedPasswords_Accounts_AccountId",
                table: "SharedPasswords",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedPasswords_SavedPasswords_SavedPasswordId",
                table: "SharedPasswords",
                column: "SavedPasswordId",
                principalTable: "SavedPasswords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedPasswords_Accounts_AccountId",
                table: "SharedPasswords");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedPasswords_SavedPasswords_SavedPasswordId",
                table: "SharedPasswords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SharedPasswords",
                table: "SharedPasswords");

            migrationBuilder.RenameTable(
                name: "SharedPasswords",
                newName: "SharedPassword");

            migrationBuilder.RenameIndex(
                name: "IX_SharedPasswords_SavedPasswordId",
                table: "SharedPassword",
                newName: "IX_SharedPassword_SavedPasswordId");

            migrationBuilder.RenameIndex(
                name: "IX_SharedPasswords_AccountId",
                table: "SharedPassword",
                newName: "IX_SharedPassword_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharedPassword",
                table: "SharedPassword",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedPassword_Accounts_AccountId",
                table: "SharedPassword",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedPassword_SavedPasswords_SavedPasswordId",
                table: "SharedPassword",
                column: "SavedPasswordId",
                principalTable: "SavedPasswords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

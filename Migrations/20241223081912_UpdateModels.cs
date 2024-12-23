using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demir.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userAgent",
                table: "Tokens",
                newName: "UserAgent");

            migrationBuilder.AlterColumn<double>(
                name: "Withdraw",
                table: "Transactions",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserAgent",
                table: "Tokens",
                newName: "userAgent");

            migrationBuilder.AlterColumn<decimal>(
                name: "Withdraw",
                table: "Transactions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");
        }
    }
}

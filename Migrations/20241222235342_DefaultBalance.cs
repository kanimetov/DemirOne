using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demir.Migrations
{
    /// <inheritdoc />
    public partial class DefaultBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Balances",
                type: "REAL",
                nullable: false,
                defaultValue: 8.0,
                oldClrType: typeof(double),
                oldType: "REAL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Balances",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldDefaultValue: 8.0);
        }
    }
}

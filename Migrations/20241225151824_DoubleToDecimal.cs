using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demir.Migrations
{
    /// <inheritdoc />
    public partial class DoubleToDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Withdraw",
                table: "Transactions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Balances",
                type: "TEXT",
                nullable: false,
                defaultValue: 8m,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldDefaultValue: 8.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Withdraw",
                table: "Transactions",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Balances",
                type: "REAL",
                nullable: false,
                defaultValue: 8.0,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldDefaultValue: 8m);
        }
    }
}

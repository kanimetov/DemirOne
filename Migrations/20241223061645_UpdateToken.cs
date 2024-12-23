using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demir.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userAgent",
                table: "Tokens",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userAgent",
                table: "Tokens");
        }
    }
}

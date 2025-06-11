using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NokhbeganApi.Migrations
{
    /// <inheritdoc />
    public partial class price_term_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "studentTerms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "studentTerms");
        }
    }
}

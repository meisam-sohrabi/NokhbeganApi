using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NokhbeganApi.Migrations
{
    /// <inheritdoc />
    public partial class globalconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDiscountPercent",
                table: "discount");

            migrationBuilder.CreateTable(
                name: "GlobalConfig",
                columns: table => new
                {
                    ID_Global = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxGlobalDiscountPercent = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalConfig", x => x.ID_Global);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalConfig");

            migrationBuilder.AddColumn<double>(
                name: "MaxDiscountPercent",
                table: "discount",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}

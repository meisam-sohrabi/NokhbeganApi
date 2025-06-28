using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NokhbeganApi.Migrations
{
    /// <inheritdoc />
    public partial class delete_description_payment_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "payments");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                table: "payments",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PaymentType",
                table: "payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "payments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

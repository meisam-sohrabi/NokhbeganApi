using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NokhbeganApi.Migrations
{
    /// <inheritdoc />
    public partial class changing_payment_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "IsSuccess",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "payments");

            migrationBuilder.RenameColumn(
                name: "paymentStatusCode",
                table: "payments",
                newName: "PaymentType");

            migrationBuilder.RenameColumn(
                name: "PayWithDiscount",
                table: "payments",
                newName: "AmountWithDiscount");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "payments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "payments");

            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "payments",
                newName: "paymentStatusCode");

            migrationBuilder.RenameColumn(
                name: "AmountWithDiscount",
                table: "payments",
                newName: "PayWithDiscount");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                table: "payments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TrackId",
                table: "payments",
                type: "bigint",
                nullable: true);
        }
    }
}

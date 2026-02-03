using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repair.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateappt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_PhoneParts_PhonePartId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "PhonePartId",
                table: "Appointments",
                newName: "RepairPriceId");

            migrationBuilder.RenameColumn(
                name: "OwnerState",
                table: "Appointments",
                newName: "CustomerState");

            migrationBuilder.RenameColumn(
                name: "OwnerPhoneNumber",
                table: "Appointments",
                newName: "CustomerPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "OwnerLastname",
                table: "Appointments",
                newName: "CustomerLastname");

            migrationBuilder.RenameColumn(
                name: "OwnerFirstName",
                table: "Appointments",
                newName: "CustomerFirstName");

            migrationBuilder.RenameColumn(
                name: "OwnerCity",
                table: "Appointments",
                newName: "CustomerCity");

            migrationBuilder.RenameColumn(
                name: "OwnerAddress",
                table: "Appointments",
                newName: "CustomerAddress");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_PhonePartId",
                table: "Appointments",
                newName: "IX_Appointments_RepairPriceId");

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Appointments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_RepairPrices_RepairPriceId",
                table: "Appointments",
                column: "RepairPriceId",
                principalTable: "RepairPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_RepairPrices_RepairPriceId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "RepairPriceId",
                table: "Appointments",
                newName: "PhonePartId");

            migrationBuilder.RenameColumn(
                name: "CustomerState",
                table: "Appointments",
                newName: "OwnerState");

            migrationBuilder.RenameColumn(
                name: "CustomerPhoneNumber",
                table: "Appointments",
                newName: "OwnerPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "CustomerLastname",
                table: "Appointments",
                newName: "OwnerLastname");

            migrationBuilder.RenameColumn(
                name: "CustomerFirstName",
                table: "Appointments",
                newName: "OwnerFirstName");

            migrationBuilder.RenameColumn(
                name: "CustomerCity",
                table: "Appointments",
                newName: "OwnerCity");

            migrationBuilder.RenameColumn(
                name: "CustomerAddress",
                table: "Appointments",
                newName: "OwnerAddress");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_RepairPriceId",
                table: "Appointments",
                newName: "IX_Appointments_PhonePartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_PhoneParts_PhonePartId",
                table: "Appointments",
                column: "PhonePartId",
                principalTable: "PhoneParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

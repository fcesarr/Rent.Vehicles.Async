using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rent.Vehicles.Entities.Migrations
{
    /// <inheritdoc />
    public partial class AlterVehicleAddRentCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("09122534-816b-43b3-bd08-51f7aa392011"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("23f77fef-715e-4b5b-810f-7aa4eb2bc32d"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("3cf83b51-5a42-4e02-92d0-3be9669d7aee"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("7a605b65-512b-48a1-824a-660fff6d15a3"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("c218db2a-9d3e-40f0-b261-1eca059b5bef"));

            migrationBuilder.AddColumn<int>(
                name: "rent_count",
                schema: "vehicles",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                schema: "vehicles",
                table: "rentalPlanes",
                columns: new[] { "id", "created", "daily_cost", "number_of_days", "post_end_date_fine", "pre_end_date_percentage_fine", "updated" },
                values: new object[,]
                {
                    { new Guid("3962fd2e-c0d7-4040-bb25-fa5d5f3a8367"), new DateTime(2024, 7, 15, 16, 49, 49, 440, DateTimeKind.Local).AddTicks(5451), 18.0m, 50, 50.0m, 1.0m, null },
                    { new Guid("5583ab72-7a4d-42b7-b72e-ab9f4fa36d27"), new DateTime(2024, 7, 15, 16, 49, 49, 440, DateTimeKind.Local).AddTicks(5429), 28.0m, 15, 50.0m, 1.40m, null },
                    { new Guid("8aa96af9-0d36-426d-87af-a2f843457265"), new DateTime(2024, 7, 15, 16, 49, 49, 440, DateTimeKind.Local).AddTicks(5361), 30.0m, 7, 50.0m, 1.20m, null },
                    { new Guid("c8bb6d3f-4e75-4525-90ad-4db54a19e7c9"), new DateTime(2024, 7, 15, 16, 49, 49, 440, DateTimeKind.Local).AddTicks(5445), 20.0m, 45, 50.0m, 1.0m, null },
                    { new Guid("e6768c11-40ff-406c-bc79-343aaaafbfb0"), new DateTime(2024, 7, 15, 16, 49, 49, 440, DateTimeKind.Local).AddTicks(5438), 22.0m, 30, 50.0m, 1.0m, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("3962fd2e-c0d7-4040-bb25-fa5d5f3a8367"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("5583ab72-7a4d-42b7-b72e-ab9f4fa36d27"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("8aa96af9-0d36-426d-87af-a2f843457265"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("c8bb6d3f-4e75-4525-90ad-4db54a19e7c9"));

            migrationBuilder.DeleteData(
                schema: "vehicles",
                table: "rentalPlanes",
                keyColumn: "id",
                keyValue: new Guid("e6768c11-40ff-406c-bc79-343aaaafbfb0"));

            migrationBuilder.DropColumn(
                name: "rent_count",
                schema: "vehicles",
                table: "vehicles");

            migrationBuilder.InsertData(
                schema: "vehicles",
                table: "rentalPlanes",
                columns: new[] { "id", "created", "daily_cost", "number_of_days", "post_end_date_fine", "pre_end_date_percentage_fine", "updated" },
                values: new object[,]
                {
                    { new Guid("09122534-816b-43b3-bd08-51f7aa392011"), new DateTime(2024, 6, 30, 14, 31, 19, 665, DateTimeKind.Utc).AddTicks(6974), 18.0m, 50, 50.0m, 1.0m, null },
                    { new Guid("23f77fef-715e-4b5b-810f-7aa4eb2bc32d"), new DateTime(2024, 6, 30, 14, 31, 19, 665, DateTimeKind.Utc).AddTicks(6908), 30.0m, 7, 50.0m, 1.20m, null },
                    { new Guid("3cf83b51-5a42-4e02-92d0-3be9669d7aee"), new DateTime(2024, 6, 30, 14, 31, 19, 665, DateTimeKind.Utc).AddTicks(6955), 28.0m, 15, 50.0m, 1.40m, null },
                    { new Guid("7a605b65-512b-48a1-824a-660fff6d15a3"), new DateTime(2024, 6, 30, 14, 31, 19, 665, DateTimeKind.Utc).AddTicks(6968), 20.0m, 45, 50.0m, 1.0m, null },
                    { new Guid("c218db2a-9d3e-40f0-b261-1eca059b5bef"), new DateTime(2024, 6, 30, 14, 31, 19, 665, DateTimeKind.Utc).AddTicks(6962), 22.0m, 30, 50.0m, 1.0m, null }
                });
        }
    }
}

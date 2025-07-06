using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoLenguajes.Migrations
{
    /// <inheritdoc />
    public partial class agregarStatusUnconfirmed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "TimeToNextStatus" },
                values: new object[] { "Unconfirmed", null });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "TimeToNextStatus" },
                values: new object[] { "On Time", 10 });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "NextStatusId", "TimeToNextStatus" },
                values: new object[] { "Over Time", 4, 15 });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Delayed");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Canceled");

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "Name", "NextStatusId", "TimeToNextStatus" },
                values: new object[] { 6, "Delivered", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "TimeToNextStatus" },
                values: new object[] { "On Time", 10 });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "TimeToNextStatus" },
                values: new object[] { "Over Time", 15 });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "NextStatusId", "TimeToNextStatus" },
                values: new object[] { "Delayed", null, null });

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Canceled");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Delivered");
        }
    }
}

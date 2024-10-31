using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone.Migrations
{
    /// <inheritdoc />
    public partial class AddEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created_at",
                table: "Customers");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created_at",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Jumpers = table.Column<int>(type: "int", nullable: false),
                    Socks = table.Column<int>(type: "int", nullable: false),
                    Addons = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrampolineGames = table.Column<int>(type: "int", nullable: false),
                    PartyGuest = table.Column<int>(type: "int", nullable: false),
                    PartyHours = table.Column<int>(type: "int", nullable: false),
                    PartyDecorations = table.Column<int>(type: "int", nullable: false),
                    ElecFoodCart = table.Column<int>(type: "int", nullable: false),
                    PartyEquipCD = table.Column<int>(type: "int", nullable: false),
                    PartyEquipUtils = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropColumn(
                name: "Created_at",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "Date_created",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

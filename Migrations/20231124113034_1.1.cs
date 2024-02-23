using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace przychodnia.Migrations
{
    public partial class _11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EndtTime",
                table: "Schedules",
                newName: "EndTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Schedules",
                newName: "EndtTime");
        }
    }
}

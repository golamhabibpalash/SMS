using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class SessionModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CurrentSession",
                table: "AcademicSession",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSession",
                table: "AcademicSession");
        }
    }
}

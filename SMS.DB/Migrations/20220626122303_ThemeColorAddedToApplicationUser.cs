using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class ThemeColorAddedToApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThemeColor",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThemeColor",
                table: "AspNetUsers");
        }
    }
}

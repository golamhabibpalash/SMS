using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class AttendanceVSApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Attendances");

            migrationBuilder.AddColumn<string>(
                name: "FavIcon",
                table: "Institute",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Attendances",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ApplicationUserId",
                table: "Attendances",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_AspNetUsers_ApplicationUserId",
                table: "Attendances",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AspNetUsers_ApplicationUserId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_ApplicationUserId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "FavIcon",
                table: "Institute");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Attendances");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "Attendances",
                type: "nvarchar(1)",
                nullable: false,
                defaultValue: "");
        }
    }
}

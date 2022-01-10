using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class ApplicationUserId_Added_To_Attedance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AspNetUsers_ApplicationUserId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_ApplicationUserId",
                table: "Attendances");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationUserId",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "Attendances",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ApplicationUserId1",
                table: "Attendances",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_AspNetUsers_ApplicationUserId1",
                table: "Attendances",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AspNetUsers_ApplicationUserId1",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_ApplicationUserId1",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "Attendances");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Attendances",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class BloodGroupNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_BloodGroup_BloodGroupId",
                table: "Student");

            migrationBuilder.AlterColumn<int>(
                name: "BloodGroupId",
                table: "Student",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_BloodGroup_BloodGroupId",
                table: "Student",
                column: "BloodGroupId",
                principalTable: "BloodGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_BloodGroup_BloodGroupId",
                table: "Student");

            migrationBuilder.AlterColumn<int>(
                name: "BloodGroupId",
                table: "Student",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_BloodGroup_BloodGroupId",
                table: "Student",
                column: "BloodGroupId",
                principalTable: "BloodGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class Employee_BloodGroup_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BloodGroupId",
                table: "Employee",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_BloodGroupId",
                table: "Employee",
                column: "BloodGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_BloodGroup_BloodGroupId",
                table: "Employee",
                column: "BloodGroupId",
                principalTable: "BloodGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_BloodGroup_BloodGroupId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_BloodGroupId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "BloodGroupId",
                table: "Employee");
        }
    }
}

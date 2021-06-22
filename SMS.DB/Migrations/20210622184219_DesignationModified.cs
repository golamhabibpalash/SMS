using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class DesignationModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpTypeId",
                table: "Designation",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Designation_EmpTypeId",
                table: "Designation",
                column: "EmpTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Designation_EmpType_EmpTypeId",
                table: "Designation",
                column: "EmpTypeId",
                principalTable: "EmpType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designation_EmpType_EmpTypeId",
                table: "Designation");

            migrationBuilder.DropIndex(
                name: "IX_Designation_EmpTypeId",
                table: "Designation");

            migrationBuilder.DropColumn(
                name: "EmpTypeId",
                table: "Designation");
        }
    }
}

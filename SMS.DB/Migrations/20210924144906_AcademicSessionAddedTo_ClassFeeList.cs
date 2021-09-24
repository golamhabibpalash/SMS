using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class AcademicSessionAddedTo_ClassFeeList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassFeeList_AcademicSession_AcademicSessionId",
                table: "ClassFeeList");

            migrationBuilder.AlterColumn<int>(
                name: "AcademicSessionId",
                table: "ClassFeeList",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassFeeList_AcademicSession_AcademicSessionId",
                table: "ClassFeeList",
                column: "AcademicSessionId",
                principalTable: "AcademicSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassFeeList_AcademicSession_AcademicSessionId",
                table: "ClassFeeList");

            migrationBuilder.AlterColumn<int>(
                name: "AcademicSessionId",
                table: "ClassFeeList",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassFeeList_AcademicSession_AcademicSessionId",
                table: "ClassFeeList",
                column: "AcademicSessionId",
                principalTable: "AcademicSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

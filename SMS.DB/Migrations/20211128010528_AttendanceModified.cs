using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class AttendanceModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AcademicClass_AcademicClassId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AcademicSession_AcademicSessionId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Student_StudentId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_AcademicClassId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_AcademicSessionId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "AcademicClassId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Attendances",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "Attendances",
                type: "nvarchar(1)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Attendances",
                newName: "StudentId");

            migrationBuilder.AddColumn<int>(
                name: "AcademicClassId",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AcademicSessionId",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_AcademicClassId",
                table: "Attendances",
                column: "AcademicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_AcademicSessionId",
                table: "Attendances",
                column: "AcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_AcademicClass_AcademicClassId",
                table: "Attendances",
                column: "AcademicClassId",
                principalTable: "AcademicClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_AcademicSession_AcademicSessionId",
                table: "Attendances",
                column: "AcademicSessionId",
                principalTable: "AcademicSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Student_StudentId",
                table: "Attendances",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

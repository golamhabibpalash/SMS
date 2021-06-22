using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class EmployeeModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Division_PermanentDivisiontId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Division_PresentDivisiontId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Division_PermanentDivisiontId",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Division_PresentDivisiontId",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_PermanentDivisiontId",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_PresentDivisiontId",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Employee_PermanentDivisiontId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_PresentDivisiontId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Institute");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Institute");

            migrationBuilder.DropColumn(
                name: "EditedAt",
                table: "Institute");

            migrationBuilder.DropColumn(
                name: "EditedBy",
                table: "Institute");

            migrationBuilder.DropColumn(
                name: "PermanentDivisiontId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "PresentDivisiontId",
                table: "Employee");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "MotherName",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FatherName",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PermanentDivisionId",
                table: "Student",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PresentDivisionId",
                table: "Student",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PresentDivisionId",
                table: "Employee",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPresent = table.Column<bool>(type: "bit", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    InTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OutTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcademicClassId = table.Column<int>(type: "int", nullable: false),
                    AcademicSessionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_AcademicClass_AcademicClassId",
                        column: x => x.AcademicClassId,
                        principalTable: "AcademicClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendances_AcademicSession_AcademicSessionId",
                        column: x => x.AcademicSessionId,
                        principalTable: "AcademicSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendances_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_PermanentDivisionId",
                table: "Student",
                column: "PermanentDivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_PresentDivisionId",
                table: "Student",
                column: "PresentDivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_PermanentDivisionId",
                table: "Employee",
                column: "PermanentDivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_PresentDivisionId",
                table: "Employee",
                column: "PresentDivisionId");

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
                name: "FK_Employee_Division_PermanentDivisionId",
                table: "Employee",
                column: "PermanentDivisionId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Division_PresentDivisionId",
                table: "Employee",
                column: "PresentDivisionId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Division_PermanentDivisionId",
                table: "Student",
                column: "PermanentDivisionId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Division_PresentDivisionId",
                table: "Student",
                column: "PresentDivisionId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Division_PermanentDivisionId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Division_PresentDivisionId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Division_PermanentDivisionId",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Division_PresentDivisionId",
                table: "Student");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Student_PermanentDivisionId",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_PresentDivisionId",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Employee_PermanentDivisionId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_PresentDivisionId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "PermanentDivisionId",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "PresentDivisionId",
                table: "Student");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Student",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MotherName",
                table: "Student",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FatherName",
                table: "Student",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Institute",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Institute",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedAt",
                table: "Institute",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                table: "Institute",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PresentDivisionId",
                table: "Employee",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PermanentDivisiontId",
                table: "Employee",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PresentDivisiontId",
                table: "Employee",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Student_PermanentDivisiontId",
                table: "Student",
                column: "PermanentDivisiontId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_PresentDivisiontId",
                table: "Student",
                column: "PresentDivisiontId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_PermanentDivisiontId",
                table: "Employee",
                column: "PermanentDivisiontId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_PresentDivisiontId",
                table: "Employee",
                column: "PresentDivisiontId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Division_PermanentDivisiontId",
                table: "Employee",
                column: "PermanentDivisiontId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Division_PresentDivisiontId",
                table: "Employee",
                column: "PresentDivisiontId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Division_PermanentDivisiontId",
                table: "Student",
                column: "PermanentDivisiontId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Division_PresentDivisiontId",
                table: "Student",
                column: "PresentDivisiontId",
                principalTable: "Division",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

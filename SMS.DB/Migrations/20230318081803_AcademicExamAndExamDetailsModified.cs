using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class AcademicExamAndExamDetailsModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExamResultId",
                table: "AcademicExamDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExamResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicExamId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MACAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamResults_AcademicExams_AcademicExamId",
                        column: x => x.AcademicExamId,
                        principalTable: "AcademicExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamResultDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ExamResultId = table.Column<int>(type: "int", nullable: false),
                    Marks = table.Column<double>(type: "float", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MACAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamResultDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamResultDetails_ExamResults_ExamResultId",
                        column: x => x.ExamResultId,
                        principalTable: "ExamResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamResultDetails_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicExamDetails_ExamResultId",
                table: "AcademicExamDetails",
                column: "ExamResultId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultDetails_ExamResultId",
                table: "ExamResultDetails",
                column: "ExamResultId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResultDetails_StudentId",
                table: "ExamResultDetails",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_AcademicExamId",
                table: "ExamResults",
                column: "AcademicExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicExamDetails_ExamResults_ExamResultId",
                table: "AcademicExamDetails",
                column: "ExamResultId",
                principalTable: "ExamResults",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicExamDetails_ExamResults_ExamResultId",
                table: "AcademicExamDetails");

            migrationBuilder.DropTable(
                name: "ExamResultDetails");

            migrationBuilder.DropTable(
                name: "ExamResults");

            migrationBuilder.DropIndex(
                name: "IX_AcademicExamDetails_ExamResultId",
                table: "AcademicExamDetails");

            migrationBuilder.DropColumn(
                name: "ExamResultId",
                table: "AcademicExamDetails");
        }
    }
}

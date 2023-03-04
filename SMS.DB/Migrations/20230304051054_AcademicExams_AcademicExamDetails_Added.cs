using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class AcademicExams_AcademicExamDetails_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicExams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcademicExamTypeId = table.Column<int>(type: "int", nullable: false),
                    AcademicSessionId = table.Column<int>(type: "int", nullable: false),
                    AcademicSubjectId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    TotalMarks = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MACAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicExams_AcademicExamTypes_AcademicExamTypeId",
                        column: x => x.AcademicExamTypeId,
                        principalTable: "AcademicExamTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcademicExams_AcademicSession_AcademicSessionId",
                        column: x => x.AcademicSessionId,
                        principalTable: "AcademicSession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcademicExams_AcademicSubject_AcademicSubjectId",
                        column: x => x.AcademicSubjectId,
                        principalTable: "AcademicSubject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcademicExams_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademicExamDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicExamId = table.Column<int>(type: "int", nullable: false),
                    ObtainMark = table.Column<double>(type: "float", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MACAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicExamDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicExamDetails_AcademicExams_AcademicExamId",
                        column: x => x.AcademicExamId,
                        principalTable: "AcademicExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcademicExamDetails_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicExamDetails_AcademicExamId",
                table: "AcademicExamDetails",
                column: "AcademicExamId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicExamDetails_StudentId",
                table: "AcademicExamDetails",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicExams_AcademicExamTypeId",
                table: "AcademicExams",
                column: "AcademicExamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicExams_AcademicSessionId",
                table: "AcademicExams",
                column: "AcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicExams_AcademicSubjectId",
                table: "AcademicExams",
                column: "AcademicSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicExams_EmployeeId",
                table: "AcademicExams",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicExamDetails");

            migrationBuilder.DropTable(
                name: "AcademicExams");
        }
    }
}

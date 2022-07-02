using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class QuestionFormationAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionDetails_QuestionTypes_QuestionTypeId",
                table: "QuestionDetails");

            migrationBuilder.DropTable(
                name: "QuestionTypes");

            migrationBuilder.DropIndex(
                name: "IX_QuestionDetails_QuestionTypeId",
                table: "QuestionDetails");

            migrationBuilder.DropColumn(
                name: "QuestionTypeId",
                table: "QuestionDetails");

            migrationBuilder.AddColumn<int>(
                name: "QuestionFormatId",
                table: "AcademicSubject",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionFormats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfQuestion = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MACAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionFormats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicSubject_QuestionFormatId",
                table: "AcademicSubject",
                column: "QuestionFormatId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicSubject_QuestionFormats_QuestionFormatId",
                table: "AcademicSubject",
                column: "QuestionFormatId",
                principalTable: "QuestionFormats",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicSubject_QuestionFormats_QuestionFormatId",
                table: "AcademicSubject");

            migrationBuilder.DropTable(
                name: "QuestionFormats");

            migrationBuilder.DropIndex(
                name: "IX_AcademicSubject_QuestionFormatId",
                table: "AcademicSubject");

            migrationBuilder.DropColumn(
                name: "QuestionFormatId",
                table: "AcademicSubject");

            migrationBuilder.AddColumn<int>(
                name: "QuestionTypeId",
                table: "QuestionDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MACAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarkValue = table.Column<double>(type: "float", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionDetails_QuestionTypeId",
                table: "QuestionDetails",
                column: "QuestionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionDetails_QuestionTypes_QuestionTypeId",
                table: "QuestionDetails",
                column: "QuestionTypeId",
                principalTable: "QuestionTypes",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class ChpaterAddedQuestionRelatedModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_AcademicClass_AcademicClassId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_AcademicSubject_AcademicSubjectId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_AcademicClassId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_AcademicSubjectId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AcademicClassId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AcademicSubjectId",
                table: "Questions");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcademicClassId",
                table: "AcademicSubject",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChapterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChapterNumber = table.Column<int>(type: "int", nullable: false),
                    AcademicSubjectId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapters_AcademicSubject_AcademicSubjectId",
                        column: x => x.AcademicSubjectId,
                        principalTable: "AcademicSubject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ChapterId",
                table: "Questions",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicSubject_AcademicClassId",
                table: "AcademicSubject",
                column: "AcademicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_AcademicSubjectId",
                table: "Chapters",
                column: "AcademicSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicSubject_AcademicClass_AcademicClassId",
                table: "AcademicSubject",
                column: "AcademicClassId",
                principalTable: "AcademicClass",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Chapters_ChapterId",
                table: "Questions",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicSubject_AcademicClass_AcademicClassId",
                table: "AcademicSubject");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Chapters_ChapterId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ChapterId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_AcademicSubject_AcademicClassId",
                table: "AcademicSubject");

            migrationBuilder.DropColumn(
                name: "AcademicClassId",
                table: "AcademicSubject");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Image",
                table: "Questions",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcademicClassId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AcademicSubjectId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_AcademicClassId",
                table: "Questions",
                column: "AcademicClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_AcademicSubjectId",
                table: "Questions",
                column: "AcademicSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AcademicClass_AcademicClassId",
                table: "Questions",
                column: "AcademicClassId",
                principalTable: "AcademicClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AcademicSubject_AcademicSubjectId",
                table: "Questions",
                column: "AcademicSubjectId",
                principalTable: "AcademicSubject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

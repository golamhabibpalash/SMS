using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class QMark_Added_To_QuestionDetails_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicSubject_AcademicClass_AcademicClassId",
                table: "AcademicSubject");

            migrationBuilder.AlterColumn<string>(
                name: "QFormat",
                table: "QuestionFormats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "QuestionFormats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "QMark",
                table: "QuestionDetails",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AcademicClassId",
                table: "AcademicSubject",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicSubject_AcademicClass_AcademicClassId",
                table: "AcademicSubject",
                column: "AcademicClassId",
                principalTable: "AcademicClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicSubject_AcademicClass_AcademicClassId",
                table: "AcademicSubject");

            migrationBuilder.DropColumn(
                name: "QMark",
                table: "QuestionDetails");

            migrationBuilder.AlterColumn<string>(
                name: "QFormat",
                table: "QuestionFormats",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "QuestionFormats",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "AcademicClassId",
                table: "AcademicSubject",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicSubject_AcademicClass_AcademicClassId",
                table: "AcademicSubject",
                column: "AcademicClassId",
                principalTable: "AcademicClass",
                principalColumn: "Id");
        }
    }
}

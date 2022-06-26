using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class QuestionTypeNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionDetails_QuestionTypes_QuestionTypeId",
                table: "QuestionDetails");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionTypeId",
                table: "QuestionDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionDetails_QuestionTypes_QuestionTypeId",
                table: "QuestionDetails",
                column: "QuestionTypeId",
                principalTable: "QuestionTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionDetails_QuestionTypes_QuestionTypeId",
                table: "QuestionDetails");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionTypeId",
                table: "QuestionDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionDetails_QuestionTypes_QuestionTypeId",
                table: "QuestionDetails",
                column: "QuestionTypeId",
                principalTable: "QuestionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

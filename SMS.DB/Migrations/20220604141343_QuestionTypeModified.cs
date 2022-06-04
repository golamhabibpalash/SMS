using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class QuestionTypeModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkNumber",
                table: "QuestionDetails");

            migrationBuilder.AddColumn<double>(
                name: "MarkValue",
                table: "QuestionTypes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkValue",
                table: "QuestionTypes");

            migrationBuilder.AddColumn<double>(
                name: "MarkNumber",
                table: "QuestionDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}

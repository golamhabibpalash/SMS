using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class AcademicSectionModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicSessionId",
                table: "AcademicSection",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicSection_AcademicSessionId",
                table: "AcademicSection",
                column: "AcademicSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicSection_AcademicSession_AcademicSessionId",
                table: "AcademicSection",
                column: "AcademicSessionId",
                principalTable: "AcademicSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicSection_AcademicSession_AcademicSessionId",
                table: "AcademicSection");

            migrationBuilder.DropIndex(
                name: "IX_AcademicSection_AcademicSessionId",
                table: "AcademicSection");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "AcademicSection");
        }
    }
}

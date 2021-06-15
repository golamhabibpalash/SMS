using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class sessionIdRemovedFromAcademicClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicClass_AcademicSession_AcademicSessionId",
                table: "AcademicClass");

            migrationBuilder.DropIndex(
                name: "IX_AcademicClass_AcademicSessionId",
                table: "AcademicClass");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "AcademicClass");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicSessionId",
                table: "AcademicClass",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicClass_AcademicSessionId",
                table: "AcademicClass",
                column: "AcademicSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicClass_AcademicSession_AcademicSessionId",
                table: "AcademicClass",
                column: "AcademicSessionId",
                principalTable: "AcademicSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

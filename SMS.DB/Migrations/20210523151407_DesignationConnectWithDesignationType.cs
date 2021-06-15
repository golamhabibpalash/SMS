using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class DesignationConnectWithDesignationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DesignationTypeId",
                table: "Designation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Designation_DesignationTypeId",
                table: "Designation",
                column: "DesignationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Designation_DesignationType_DesignationTypeId",
                table: "Designation",
                column: "DesignationTypeId",
                principalTable: "DesignationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Designation_DesignationType_DesignationTypeId",
                table: "Designation");

            migrationBuilder.DropIndex(
                name: "IX_Designation_DesignationTypeId",
                table: "Designation");

            migrationBuilder.DropColumn(
                name: "DesignationTypeId",
                table: "Designation");
        }
    }
}

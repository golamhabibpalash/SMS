using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class tran_MachineRawPunch_Name_Modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tran_MachineRawPunch",
                table: "Tran_MachineRawPunch",
                column: "Tran_MachineRawPunchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tran_MachineRawPunch",
                table: "Tran_MachineRawPunch");

            migrationBuilder.RenameTable(
                name: "Tran_MachineRawPunch",
                newName: "Tran_MachineRawPunche");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tran_MachineRawPunche",
                table: "Tran_MachineRawPunche",
                column: "Tran_MachineRawPunchId");
        }
    }
}

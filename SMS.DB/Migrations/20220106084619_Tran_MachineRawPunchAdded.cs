using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class Tran_MachineRawPunchAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tran_MachineRawPunche",
                columns: table => new
                {
                    Tran_MachineRawPunchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PunchDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    P_Day = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    ISManual = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    PayCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MachineNo = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tran_MachineRawPunche", x => x.Tran_MachineRawPunchId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tran_MachineRawPunche");
        }
    }
}

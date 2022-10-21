using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class setupMobileSMS_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SetupMobileSMSs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SMSService = table.Column<bool>(type: "bit", nullable: false),
                    AttendanceSMSService = table.Column<bool>(type: "bit", nullable: false),
                    CheckInSMSService = table.Column<bool>(type: "bit", nullable: false),
                    CheckInSMSServiceForMaleStudent = table.Column<bool>(type: "bit", nullable: false),
                    CheckInSMSServiceForGirlsStudent = table.Column<bool>(type: "bit", nullable: false),
                    CheckInSMSServiceForEmployees = table.Column<bool>(type: "bit", nullable: false),
                    CheckOutSMSService = table.Column<bool>(type: "bit", nullable: false),
                    CheckOutSMSServiceForMaleStudent = table.Column<bool>(type: "bit", nullable: false),
                    CheckOutSMSServiceForGirlsStudent = table.Column<bool>(type: "bit", nullable: false),
                    CheckOutSMSServiceForEmployees = table.Column<bool>(type: "bit", nullable: false),
                    PaymentSMSService = table.Column<bool>(type: "bit", nullable: false),
                    AdministrativeSMSService = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MACAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetupMobileSMSs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SetupMobileSMSs");
        }
    }
}

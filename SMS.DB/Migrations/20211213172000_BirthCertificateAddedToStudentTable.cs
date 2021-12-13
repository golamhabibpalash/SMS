using Microsoft.EntityFrameworkCore.Migrations;

namespace SMS.DB.Migrations
{
    public partial class BirthCertificateAddedToStudentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BirthCertificateImage",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirthCertificateNo",
                table: "Student",
                type: "nvarchar(17)",
                maxLength: 17,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthCertificateImage",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "BirthCertificateNo",
                table: "Student");
        }
    }
}

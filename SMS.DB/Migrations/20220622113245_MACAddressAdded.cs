using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class MACAddressAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Upazila",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "StudentPaymentDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "StudentPayment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "StudentFeeHead",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Religion",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "QuestionTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "QuestionDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "PhoneSMS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Nationality",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Institute",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Gender",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "EmpType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Division",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "District",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "DesignationType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Designation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "ClassFeeList",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Chapters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "BloodGroup",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "AttachDocs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "AcademicSubjectType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "AcademicSubject",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "AcademicSession",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "AcademicSection",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "AcademicClass",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Upazila");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "StudentPaymentDetails");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "StudentPayment");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "StudentFeeHead");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Religion");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "QuestionTypes");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "QuestionDetails");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "PhoneSMS");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Nationality");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Institute");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Gender");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "EmpType");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Division");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "District");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "DesignationType");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Designation");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "ClassFeeList");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "BloodGroup");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "AttachDocs");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "AcademicSubjectType");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "AcademicSubject");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "AcademicSession");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "AcademicSection");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "AcademicClass");
        }
    }
}

﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    /// <inheritdoc />
    public partial class uniqueContraintAddedInProjectModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ModuleName",
                table: "ProjectModules",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectModules_ModuleName",
                table: "ProjectModules",
                column: "ModuleName",
                unique: true,
                filter: "[ModuleName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectModules_ModuleName",
                table: "ProjectModules");

            migrationBuilder.AlterColumn<string>(
                name: "ModuleName",
                table: "ProjectModules",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}

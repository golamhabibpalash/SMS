using Microsoft.EntityFrameworkCore.Migrations;
using SMS.Entities;
using System;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class SP_Added_GetStudentActivateHistById : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = @"USE [SMSDB]
                            GO
                            SET ANSI_NULLS ON
                            GO
                            SET QUOTED_IDENTIFIER ON
                            GO
                            CREATE PROCEDURE [dbo].[GetStudentActivateHistById]
	                            @studentId int,
	                            @targetDate datetime2
                            AS
                            BEGIN
	                            SELECT t.* FROM StudentActivateHists t WHERE @studentId = t.StudentId and t.ActionDateTime < @targetDate
                            END";
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sql = @"drop procedure GetStudentActivateHistById";
            migrationBuilder.Sql(sql);
        }
    }
}

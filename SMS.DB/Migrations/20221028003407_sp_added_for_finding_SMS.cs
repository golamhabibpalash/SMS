using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class sp_added_for_finding_SMS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = @"USE [SMSDB]
                            GO
                            /****** Object:  StoredProcedure [dbo].[sp_Get_SMS_By_phone_type_date]    Script Date: 10/28/2022 6:32:49 AM ******/
                            SET ANSI_NULLS ON
                            GO
                            SET QUOTED_IDENTIFIER ON
                            GO
                            -- =============================================
                            -- Author:		<Author,,Name>
                            -- Create date: <Create Date,,>
                            -- Description:	<Description,,>
                            -- =============================================
                            CREATE PROCEDURE [dbo].[sp_Get_SMS_By_phone_type_date] 
	                            @phone nvarchar(MAX),
	                            @smsType nvarchar(15),
	                            @smsDate varchar(8)
                            AS
                            BEGIN
	                            SELECT p.* FROM PhoneSMS p
	                            Where p.MobileNumber = @phone
	                            and (p.SMSType is not null
	                            and p.SMSType = @smsType)
	                            and FORMAT(p.CreatedAt,'yyyyMMdd') = @smsDate;
                            END
                            ";
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sql = @"drop procedure sp_Get_SMS_By_phone_type_date";
            migrationBuilder.Sql(sql);
        }
    }
}

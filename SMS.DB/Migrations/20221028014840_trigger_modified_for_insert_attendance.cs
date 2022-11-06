using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.DB.Migrations
{
    public partial class trigger_modified_for_insert_attendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = @"USE [SMSDB]
							GO
							/****** Object:  Trigger [dbo].[SendSMS]    Script Date: 10/28/2022 7:26:51 AM ******/
							SET ANSI_NULLS ON
							GO
							SET QUOTED_IDENTIFIER ON
							GO
							-- =============================================
							-- Author:		<Author,,Name>
							-- Create date: <Create Date,,>
							-- Description:	<Description,,>
							-- =============================================
							CREATE TRIGGER [dbo].[SendSMS] ON [dbo].[Tran_MachineRawPunch] 
							FOR INSERT
							AS 
							BEGIN
								SET NOCOUNT ON;
								Declare @Id varchar(15)
								--Declare @ResponseText varchar(Max)
								Declare @ApiLink varchar(Max)
								--Declare @Body varchar(Max)
								--Declare @Object int

								set @Id = (SELECT a.Tran_MachineRawPunchId from (SELECT * FROM INSERTED) a);
								SET @ApiLink = 'https://ims.nobleschoolbd.com/api/api_attendance/sendSMS?Tran_MachineRawPunchId='+@Id
								Declare @Object as Int;
								Declare @ResponseText as Varchar(8000);

								--Code Snippet
								Exec sp_OACreate 'MSXML2.XMLHTTP', @Object OUT;
								Exec sp_OAMethod @Object, 'open', NULL, 'get',@ApiLink, --Your Web Service Url (invoked)
												 'false'
								Exec sp_OAMethod @Object, 'send'
								Exec sp_OAMethod @Object, 'responseText', @ResponseText OUTPUT

								Select @ResponseText
	
								Exec sp_OADestroy @Object
							END
							";
			migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			string sql = @"DROP TRIGGER SendSMS";
			migrationBuilder.Sql(sql);
        }
    }
}

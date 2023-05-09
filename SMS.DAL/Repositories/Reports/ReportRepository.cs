﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts.Reports;
using SMS.DB;
using SMS.Entities.RptModels;
using SMS.Entities.RptModels.AttendanceVM;
using SMS.Entities.RptModels.StudentPayment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories.Reports
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RptAdmitCardVM>> GetAdmitCard(int monthId, int academicClassId, int academicSectionId)
        {
            string query = @"select t.* from vw_rpt_Admit_Card_Info t
where t.monthId = " + monthId+" and t.academicClassId = "+academicClassId+" and t.AcademicSectionId ="+academicSectionId+"";
            if (academicSectionId<=0)
            {
                query = @"select t.* from vw_rpt_Admit_Card_Info t
where t.monthId = " + monthId + " and t.academicClassId = " + academicClassId + "";
            }
            List<RptAdmitCardVM> result = await _context.RptAdmitCardVMs.FromSqlRaw(query).ToListAsync();
            return result;
        }

        public async Task<List<rptStudentPaymentsVM>> GetStudentPaymentsByRoll(int classRoll,string fromDate, string toDate)
        {
            string query = $"sp_Payments_get_all_by_roll {classRoll},'"+fromDate+"','"+toDate+"'";
            List<rptStudentPaymentsVM> rptStudentPaymentsVMs = null;

            var result = await _context.RptStudentPaymetnsVMs.FromSqlRaw(query).ToListAsync();
            rptStudentPaymentsVMs = result;
            return rptStudentPaymentsVMs;
        }

        public async Task<List<RptStudentVM>> getStudentsInfo(int AcademicSessionId, int? AcademicClassId, int? AcademicSectionId)
        {

            var pAcademicSessionId = new SqlParameter("AcademicSessionId", AcademicSessionId);
            var pAcademicClassId = new SqlParameter("@AcademicClassId", (object)AcademicClassId ?? DBNull.Value);
            var pAcademicSectionId = new SqlParameter("@AcademicSectionId", (object)AcademicSectionId ?? DBNull.Value);
            List<RptStudentVM> rptStudentVMs = null;
            try
            {
                rptStudentVMs = await _context.RptStudentVMs
                    .FromSqlInterpolated($"EXECUTE sp_rpt_Get_Students_List {pAcademicSessionId}, {pAcademicClassId}, {pAcademicSectionId}")
                    .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return rptStudentVMs;
        }
        public async Task<List<RptStudentsPaymentVM>> GetStudentPayment(string fromDate, string ToDate, string AcademicClassId, string AcademicSectionId)
        {
            List<RptStudentsPaymentVM> rptStudentsPayments = new List<RptStudentsPaymentVM>();            

            AcademicClassId = string.IsNullOrEmpty(AcademicClassId) ? "null" : "'"+AcademicClassId+"'";
            AcademicSectionId = string.IsNullOrEmpty(AcademicSectionId) ? "null" : "'"+AcademicSectionId+"'";
            string query = $"sp_Payments_Get_All_by_Date '{fromDate}','{ToDate}',{AcademicClassId},{AcademicSectionId}";
            try
            {
                rptStudentsPayments = await _context.RptStudentsPaymetnsVMs.FromSqlRaw(query).ToListAsync();
            }
            catch (Exception e)
            {
                throw;
            }
            return rptStudentsPayments;
        }

        public Task<List<RptDailyAttendaceVM>> GetDailyAttendanceReport(string fromDate, string AcademicClassId, string AcademicSectionId, string attendanceType)
        {
            throw new NotImplementedException();
        }
    }
}
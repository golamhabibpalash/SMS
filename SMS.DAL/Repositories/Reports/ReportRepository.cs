using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts.Reports;
using SMS.DB;
using SMS.Entities.RptModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<rptStudentPaymentsVM>> GetStudentPaymentsByRoll(int classRoll)
        {
            string query = "exec sp_Get_Payments_by_Roll "+classRoll;
            List<rptStudentPaymentsVM> rptStudentPaymentsVMs = null;

            var result = await _context.RptStudentPaymetnsVMs.FromSqlRaw(query).ToListAsync();
            rptStudentPaymentsVMs = result;
            return rptStudentPaymentsVMs;
        }

        public async Task<List<RptStudentVM>> getStudentsInfo()
        {
            string query = @"select r.ClassRoll, r.StudentName,r.ClassName,r.SectionName,r.SessionName,r.FatherName,r.MotherName,r.GuardianPhone,r.PhoneNo,r.BloodGroup,r.Gender,r.Religion,Case r.Status when 1 then 'Active' else 'Inactive' end Status from vw_rpt_student_info r";
            List<RptStudentVM> rptStudentVMs = new List<RptStudentVM>();
            var result =await _context.RptStudentVMs.FromSqlRaw(query).ToListAsync();
            rptStudentVMs = result;
            return rptStudentVMs;
        }
    }
}

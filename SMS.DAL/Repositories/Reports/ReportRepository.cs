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
        public async Task<List<RptStudentVM>> getStudentsInfo()
        {
            string query = "select ClassRoll, StudentName,ClassName,SessionName,FatherName,MotherName,GuardianPhone,PhoneNo,Gender,Religion,Status from vw_rpt_student_info";
            List<RptStudentVM> rptStudentVMs = new List<RptStudentVM>();
            var result =await _context.RptStudentVMs.FromSqlRaw(query).ToListAsync();
            rptStudentVMs = result;
            return rptStudentVMs;
        }
    }
}

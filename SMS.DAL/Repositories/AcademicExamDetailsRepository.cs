using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class AcademicExamDetailsRepository : Repository<AcademicExamDetail>, IAcademicExamDetailsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AcademicExamDetailsRepository(ApplicationDbContext context):base(context)
        {
            _dbContext = context;
        }
        public async Task<List<AcademicExamDetail>> GetByExamIdAsync(int examId)
        {
            var examDetails =await _dbContext.AcademicExamDetails.Where(s => s.AcademicExamId == examId).ToListAsync();
            return examDetails;
        }
        public async Task<List<AcademicExamDetail>> GetAllByExamGroupAndStudentId(int examGroupId,int studentId)
        {
            try
            {
                //var pExamGroupId = new SqlParameter("examGroupId", examGroupId);
                //var pStudentId = new SqlParameter("date", studentId);
                //var examDetails = await _dbContext.AcademicExamDetails.FromSqlInterpolated($"sp_get_examdetails_by_examGroupId_studentId {pExamGroupId},{pStudentId}").ToListAsync();
                var examDetails = await _dbContext.AcademicExamDetails
                    .Include(s => s.AcademicExam)
                    .Include(s => s.Student)
                    .Where( s=> s.AcademicExam.AcademicExamGroupId == examGroupId && s.StudentId == studentId)
                    .ToListAsync();
                return examDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

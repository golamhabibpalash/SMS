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
    public class ExamResultRepository:Repository<ExamResult>,IExamResultRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ExamResultRepository(ApplicationDbContext context):base(context)
        {
            _dbContext = context;
        }

        public async Task<List<ExamResult>> GetExamResultsByExamGroupNClassId(int examGroupId, int classId)
        {
            List<ExamResult> examResults = new List<ExamResult>();
            try
            {
                examResults = await _dbContext
                    .ExamResults
                    .Include(s => s.ExamResultDetails)
                        .ThenInclude(m => m.AcademicSubject)
                    .Include(s => s.Student)
                        .ThenInclude(s => s.AcademicClass)
                     .Include(s => s.Student.Gender)
                    .Where(s => s.AcademicExamGroupId == examGroupId && s.AcademicClassId == classId)
                    .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return examResults;
        }

        public bool IsResultProcessedAsync(int examGroupId, int classId)
        {
            bool isExist = _dbContext.ExamResults.Any(s => s.AcademicExamGroupId == examGroupId && s.AcademicClassId == classId);
            return isExist;
        }

    }
}

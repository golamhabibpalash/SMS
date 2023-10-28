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
    public class AcademicExamRepository : Repository<AcademicExam>, IAcademicExamRepository
    {
        private new readonly ApplicationDbContext _context;
        public AcademicExamRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public override Task<bool> AddAsync(AcademicExam entity)
        {
            return base.AddAsync(entity);
        }

        public override async Task<IReadOnlyCollection<AcademicExam>> GetAllAsync()
        {
            var result = await _context.AcademicExams
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicExamGroup)
                .Include(s => s.AcademicSubject)
                .Include(s => s.AcademicSection)
                .Include(s => s.Employee)
                .Include(s => s.AcademicExamDetails)
                .ToListAsync();
            return result;
        }

        public override async Task<AcademicExam> GetByIdAsync(int id)
        {
            var result = await _context.AcademicExams
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicExamGroup)
                    .ThenInclude(s => s.AcademicSession)
                .Include(s => s.AcademicSubject)
                .Include(s => s.Employee)
                .Include(s => s.AcademicSection)
                .Include(s => s.AcademicExamDetails.Where(s => s.Student.Status==true).OrderBy(s => s.Student.ClassRoll))
                    .ThenInclude(s => s.Student)
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();

            return result;
        }
        public async Task<List<AcademicExam>> GetByClassIdExamGroupId(int examGroupId, int academicClassId)
        {
            var exams = await _context.AcademicExams
                .Where(s => s.AcademicClassId == academicClassId && s.AcademicExamGroupId == examGroupId)
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicExamGroup)
                .Include(s => s.AcademicSubject)
                .Include(s => s.AcademicExamDetails)
                    .ThenInclude(m => m.Student)
                .ToListAsync();

            return exams;
        }

        
    }
}

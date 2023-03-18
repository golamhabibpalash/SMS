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
        public AcademicExamRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public override async Task<IReadOnlyCollection<AcademicExam>> GetAllAsync()
        {
            var result = await _context.AcademicExams
                .Include(s => s.AcademicExamType)
                .Include(s => s.AcademicSubject)
                    .ThenInclude(m => m.AcademicClass)
                .Include(s => s.AcademicSession)
                .Include(s => s.Employee)
                .ToListAsync();
            return result;
        }
    }
}

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
        private readonly ApplicationDbContext _context;
        public AcademicExamDetailsRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<List<AcademicExamDetail>> GetByExamIdAsync(int examId)
        {
            var examDetails =await _context.AcademicExamDetails.Where(s => s.AcademicExamId == examId).ToListAsync();
            return examDetails;
        }
    }
}

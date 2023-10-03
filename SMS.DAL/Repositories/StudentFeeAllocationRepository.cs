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
    public class StudentFeeAllocationRepository:Repository<StudentFeeAllocation>, IStudentFeeAllocationRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public StudentFeeAllocationRepository(ApplicationDbContext context):base(context)
        {
            _dbContext = context;
        }

        public override async Task<IReadOnlyCollection<StudentFeeAllocation>> GetAllAsync()
        {
            var result = await _context.StudentFeeAllocations
                .Include(s => s.Student)
                .ThenInclude(s => s.AcademicClass)
                .Include(s => s.Student.AcademicSession)
                .Include(s => s.StudentFeeHead)
                .ToListAsync();
            return result;
        }
    }
}

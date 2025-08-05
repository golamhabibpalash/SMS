using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class StudentFeeAllocationRepository : Repository<StudentFeeAllocation>, IStudentFeeAllocationRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public StudentFeeAllocationRepository(ApplicationDbContext context) : base(context)
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

        public async Task<StudentFeeAllocation> GetStudentFeeAllocationByUniqueIdClassFeeId(string uniqueId, int classfeeId)
        {
            return await _context.StudentFeeAllocations.FirstOrDefaultAsync(f => f.UniqueId == uniqueId && f.ClassFeeListId == classfeeId);
        }

        public async Task<StudentFeeAllocation> GetStudentFeeAllocationByUniqueIdFeeHeadId(string uniqueId, int feeHeadId)
        {
            var result = await _context.StudentFeeAllocations.FirstOrDefaultAsync(s => s.UniqueId == uniqueId && s.StudentFeeHeadId == feeHeadId);
            return result;
        }

        public async Task<List<StudentFeeAllocation>> GetStudentFeeAllocationByUniqueIdSessionId(string uniqueId, int sessionId)
        {
            var results = await _context.StudentFeeAllocations.Include(s => s.StudentFeeHead).Where(s => s.UniqueId == uniqueId && s.ClassFeeList.AcademicSessionId == sessionId).ToListAsync();
            return results;
        }
    }
}

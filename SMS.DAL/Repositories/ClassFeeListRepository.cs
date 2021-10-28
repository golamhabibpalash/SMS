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
    public class ClassFeeListRepository : Repository<ClassFeeList>, IClassFeeListRepository
    {
        public ClassFeeListRepository(ApplicationDbContext context) : base(context)
        {
            
        }
        public override async Task<IReadOnlyCollection<ClassFeeList>> GetAllAsync()
        {
            var feeLists = await _context
                .ClassFeeList
                .Include(cf => cf.AcademicClass)
                .Include(cf => cf.AcademicSession)
                .Include(cf => cf.StudentFeeHead)
                .ToListAsync();
            return feeLists;
        }

        public async Task<List<ClassFeeList>> GetAllByClassIdAsync(int classId)
        {
            var result = await _context.ClassFeeList.Include(c => c.StudentFeeHead).Where(c => c.AcademicClassId == classId).ToListAsync();
            return result;
        }

        public async Task<ClassFeeList> GetByClassIdAndFeeHeadIdAsync(int classId, int feeHeadId)
        {
            var feeListExist = await _context.ClassFeeList
                .FirstOrDefaultAsync(s => s.AcademicClassId == classId && s.StudentFeeHeadId == feeHeadId);

            return feeListExist;
        }
    }
}

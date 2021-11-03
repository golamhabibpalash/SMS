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
    public class StudentFeeHeadRepository : Repository<StudentFeeHead>, IStudentFeeHeadRepository
    {
        public StudentFeeHeadRepository(ApplicationDbContext db): base(db)
        {

        }
        public async Task<StudentFeeHead> GetByNameAsync(string name)
        {
            var feeHead = await _context.StudentFeeHead.FirstOrDefaultAsync(s => s.Name.ToLower().Trim() == name.ToLower().Trim());
            return feeHead;
        }

    }
}

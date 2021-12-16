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
    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext context):base(context)
        {

        }
        public override async Task<IReadOnlyCollection<Attendance>> GetAllAsync()
        {
            return await _context.Attendances.Include(a => a.ApplicationUser).ToListAsync();
        }
    }
}

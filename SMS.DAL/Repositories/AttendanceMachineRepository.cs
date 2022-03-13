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
    public class AttendanceMachineRepository : Repository<Tran_MachineRawPunch>, IAttendanceMachineRepository
    {
        public AttendanceMachineRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<List<Tran_MachineRawPunch>> GetAllAttendanceByDateAsync(DateTime dateTime)
        {
            return await _context.Tran_MachineRawPunch
                .Where(t => t.PunchDatetime.Date == dateTime.Date)
                .ToListAsync();
        }

        public async Task<Tran_MachineRawPunch> GetTodaysAttendanceByUserIdAsync(int attendanceId)
        {
            var allAttendance = await _context.Tran_MachineRawPunch.Where(t => t.PunchDatetime.Date == DateTime.Now.Date).ToListAsync();
            var existAttendance = await _context.Tran_MachineRawPunch.FirstOrDefaultAsync(m => m.CardNo == attendanceId.ToString() && m.PunchDatetime.Date == DateTime.Now.Date);

            return existAttendance;
        }
    }
}

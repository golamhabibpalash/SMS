﻿using Microsoft.EntityFrameworkCore;
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
        public override async Task<Attendance> GetByIdAsync(int id)
        {
            return await _context.Attendances.Include(a => a.ApplicationUser).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Attendance>> GetTodaysAllAttendanceAsync()
        {
            return await _context.Attendances
                .Include(a => a.ApplicationUser)
                .Where(a => a.PunchDatetime.Date == DateTime.Today.Date)
                .ToListAsync();
        }
    }
}

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
    public class BloodGroupRepository : Repository<BloodGroup>, IBloodGroupRepository
    {
        public BloodGroupRepository(ApplicationDbContext db) : base(db)
        {

        }
        public override async Task<IReadOnlyCollection<BloodGroup>> GetAllAsync()
        {
            return await _context.BloodGroup.OrderBy(b => b.Name).ToListAsync();
        }
    }
}

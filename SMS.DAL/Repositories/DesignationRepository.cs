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
    public class DesignationRepository : Repository<Designation>, IDesignationRepository
    {
        public DesignationRepository(ApplicationDbContext db): base(db)
        {

        }
        public override async Task<IReadOnlyCollection<Designation>> GetAllAsync()
        {
            return await _context.Designation
                .Include(d => d.DesignationType)
                .Include(d => d.EmpType)
                .ToListAsync();
            
        }

        public async Task<IReadOnlyCollection<Designation>> GetByEmpType(int id)
        {
            return await _context.Designation.Where(d => d.EmpTypeId == id).ToListAsync();
        }
    }
}

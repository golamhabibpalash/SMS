using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Contracts.Base;
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
    public class InstituteRepository:Repository<Institute>,IInstituteRepository
    {
        public InstituteRepository(ApplicationDbContext db):base(db)
        {

        }

        public async Task<Institute> GetFirstOrDefaultAsync()
        {
            return await _context.Institute.FirstOrDefaultAsync();
        }
    }
}

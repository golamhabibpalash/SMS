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
    public class ClaimStoreRepository:Repository<ClaimStores>, IClaimStoreRepository
    {
        public ClaimStoreRepository(ApplicationDbContext context):base(context)
        {
            
        }
        public override async Task<IReadOnlyCollection<ClaimStores>> GetAllAsync()
        {
            var claims = await _context.ClaimStores.Include(s => s.SubModule).ThenInclude(m => m.ProjectModule).ToListAsync();
            return claims;
        }
    }
}

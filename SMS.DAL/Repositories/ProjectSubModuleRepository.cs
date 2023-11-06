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
    public class ProjectSubModuleRepository:Repository<ProjectSubModule>, IProjectSubModuleRepository
    {
        public ProjectSubModuleRepository(ApplicationDbContext context):base(context)
        {
            
        }
        public override async Task<IReadOnlyCollection<ProjectSubModule>> GetAllAsync()
        {
            var subModules = await _context.ProjectSubModules.Include(s => s.ProjectModule).ToListAsync();
            return subModules;
        }
        public override async Task<ProjectSubModule> GetByIdAsync(int id)
        {
            var subModule = await _context.ProjectSubModules.Include(s => s.ClaimStoresList).FirstOrDefaultAsync(s => s.Id==id);
            return subModule;
        }
    }
}

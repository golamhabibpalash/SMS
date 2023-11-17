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
    public class ProjectModuleRepository : Repository<ProjectModule>, IProjectModuleRepository
    {
        public ProjectModuleRepository(ApplicationDbContext context) : base(context)
        {

        }
        public override async Task<ProjectModule> GetByIdAsync(int id)
        {
            ProjectModule projectModule;
            try
            {
                projectModule = await _context.ProjectModules.Include(s => s.SubModuleList).Where(s => s.Id == id).FirstOrDefaultAsync();
                return projectModule;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override async Task<IReadOnlyCollection<ProjectModule>> GetAllAsync()
        {
            var result = await _context.ProjectModules
                .Include(s => s.SubModuleList)
                .ThenInclude(s => s.ClaimStoresList)
                .ToListAsync();
            return result;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            var appUsers = await _context.ApplicationUsers.ToListAsync();
            return appUsers;
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            var appUser = await _context.ApplicationUsers.FirstOrDefaultAsync(a => a.Id == id);
            return appUser;
        }

        public async Task<ApplicationUser> GetByReferenceIdAsync(int id)
        {
            var appUser = await _context.ApplicationUsers.FirstOrDefaultAsync(a => a.ReferenceId == id);
            return appUser;
        }
    }
}

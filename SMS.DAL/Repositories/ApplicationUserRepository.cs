using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
    public class ApplicationUserRepository : IdentityDbContext<ApplicationUser>, IApplicationUserRepository
    {

        private readonly ApplicationDbContext _context;

        public async Task<ApplicationUser> GetAppUserByUserIdAsync(string id)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
    }
}

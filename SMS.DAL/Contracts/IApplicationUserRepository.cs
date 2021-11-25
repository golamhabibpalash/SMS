using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<ApplicationUser> GetByReferenceIdAsync(int id);
        Task<List<ApplicationUser>> GetAllAsync();

    }
}

using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IApplicationUserManager
    {
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<ApplicationUser> GetByReferenceIdAsync(int id);
        Task<List<ApplicationUser>> GetAllAsync();
    }
}

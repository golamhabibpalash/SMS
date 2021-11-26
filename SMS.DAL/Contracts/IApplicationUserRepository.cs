using Microsoft.AspNetCore.Identity;
using SMS.DAL.Contracts.Base;
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
        Task<ApplicationUser> GetAppUserByUserIdAsync(string id);
    }
}

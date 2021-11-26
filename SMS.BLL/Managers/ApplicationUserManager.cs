using BLL.Managers.Base;
using Microsoft.AspNetCore.Identity;
using SMS.BLL.Contracts;
using SMS.DAL.Repositories;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class ApplicationUserManager : IApplicationUserManager
    {
        private readonly ApplicationUserRepository _applicationUserRepository;
        //public ApplicationUserManager(ApplicationUser appUser) :base(appUser)
        //{
            
        //}

        public async Task<ApplicationUser> GetAppUserByUserIdAsync(string id)
        {
            return await _applicationUserRepository.GetAppUserByUserIdAsync(id);
        }
    }
}

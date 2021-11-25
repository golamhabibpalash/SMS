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

        public ApplicationUserManager(ApplicationUserRepository applicationUserRepository)
        {
            _applicationUserRepository = applicationUserRepository;
        }

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            return await _applicationUserRepository.GetAllAsync();
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _applicationUserRepository.GetByIdAsync(id);
        }

        public async Task<ApplicationUser> GetByReferenceIdAsync(int id)
        {
            return await _applicationUserRepository.GetByReferenceIdAsync(id);
        }
    }
}

using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class InstituteManager:Manager<Institute>,IInstituteManager
    {
        private readonly IInstituteRepository _instituteRepository;
        public InstituteManager(IInstituteRepository instituteRepository):base(instituteRepository)
        {
            _instituteRepository = instituteRepository;
        }

        public async Task<Institute> GetFirstOrDefaultAsync()
        {
            return await _instituteRepository.GetFirstOrDefaultAsync();
        }
    }
}

using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class EmployeeActivateHistManager : Manager<EmployeeActivateHist>, IEmployeeActivateHistManager
    {
        private readonly IEmployeeActivateHistRepository _employeeActivateHistRepository;
        public EmployeeActivateHistManager(IEmployeeActivateHistRepository repository) : base(repository)
        {
            _employeeActivateHistRepository = repository;
        }

        public async Task<bool> IsStudentActive(int id, string date)
        {
            return await _employeeActivateHistRepository.IsActiveByDateAsync(id, date);
        }
    }
}

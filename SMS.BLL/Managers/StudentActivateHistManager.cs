using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.DAL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class StudentActivateHistManager : Manager<StudentActivateHist>, IStudentActivateHistManager
    {
        private readonly IStudentActivateHistRepository _studentActivateHistRepository;
        public StudentActivateHistManager(IStudentActivateHistRepository repository) : base(repository)
        {
            _studentActivateHistRepository = repository;
        }

        public async Task<bool> IsStudentActive(int id, string date)
        {
            return await _studentActivateHistRepository.IsStudentActive(id, date);
        }
        
    }
}

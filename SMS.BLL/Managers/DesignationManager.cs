using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class DesignationManager : Manager<Designation>, IDesignationManager
    {
        IDesignationRepository _designationRepository;
        public DesignationManager(IDesignationRepository designationRepository): base(designationRepository)
        {
            _designationRepository = designationRepository;
        }
        public override Task<IReadOnlyCollection<Designation>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public async Task<IReadOnlyCollection<Designation>> GetByEmpType(int id)
        {
            return await _designationRepository.GetByEmpType(id);
        }
    }
}

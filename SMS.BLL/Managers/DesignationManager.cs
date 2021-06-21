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
        public DesignationManager(IDesignationRepository designationRepository): base(designationRepository)
        {

        }
        public override Task<IReadOnlyCollection<Designation>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }
    }
}

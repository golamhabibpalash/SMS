using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class DesignationTypeManager : Manager<DesignationType>, IDesignationTypeManager
    {
        public DesignationTypeManager(IDesignationTypeRepository designationTypeRepository) : base(designationTypeRepository)
        {

        }
    }
}

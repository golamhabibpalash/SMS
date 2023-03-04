using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.BLL.Contracts.Base;
using SMS.DAL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class AcademicExamTypeManager:Manager<AcademicExamType>,IAcademicExamTypeManager
    {
        private readonly IAcademicExamTypeRepository _academicExamTypeReporsitory;
        public AcademicExamTypeManager(IAcademicExamTypeRepository academicExamTypeReporsitory):base(academicExamTypeReporsitory)
        {
            _academicExamTypeReporsitory = academicExamTypeReporsitory;
        }

    }
}

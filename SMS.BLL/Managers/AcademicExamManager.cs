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
    public class AcademicExamManager:Manager<AcademicExam>, IAcademicExamManager
    {
        private readonly IAcademicExamRepository _academicExamRepository;
        public AcademicExamManager(IAcademicExamRepository academicExamRepository):base(academicExamRepository)
        {
            _academicExamRepository = academicExamRepository;
        }
    }
}

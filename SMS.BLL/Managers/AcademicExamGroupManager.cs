using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class AcademicExamGroupManager:Manager<AcademicExamGroup>,IAcademicExamGroupManager
    {
        public AcademicExamGroupManager(IAcademicExamGroupRepository academicExamGroupRepository):base(academicExamGroupRepository)
        {
            
        }
    }
}

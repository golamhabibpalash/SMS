using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class AcademicClassManager:Manager<AcademicClass>, IAcademicClassManager
    {
        public AcademicClassManager(IAcademicClassRepository academicClassRepository):base(academicClassRepository)
        {

        }
    }
}

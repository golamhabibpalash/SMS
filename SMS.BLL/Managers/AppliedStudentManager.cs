using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class AppliedStudentManager : Manager<AppliedStudent>, IAppliedStudentManager
    {
        public AppliedStudentManager(IAppliedStudentRepository _appliedStudentRepository) : base(_appliedStudentRepository)
        {

        }
    }
}

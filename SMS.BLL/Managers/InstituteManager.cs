using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class InstituteManager:Manager<Institute>,IInstituteManager
    {
        public InstituteManager(IInstituteRepository instituteRepository):base(instituteRepository)
        {

        }
    }
}

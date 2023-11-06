using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class ProjectModuleManager:Manager<ProjectModule>, IProjectModuleManager
    {
        public ProjectModuleManager(IProjectModuleRepository projectModuleRepository):base(projectModuleRepository)
        {
            
        }
    }
}

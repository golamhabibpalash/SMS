using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class ProjectSubModuleManager:Manager<ProjectSubModule>, IProjectSubModuleManager
    {
        public ProjectSubModuleManager(IProjectSubModuleRepository projectSubModuleRepository) :base(projectSubModuleRepository)
        {
            
        }
    }
}

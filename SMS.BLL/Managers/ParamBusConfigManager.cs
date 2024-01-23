using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class ParamBusConfigManager : Manager<ParamBusConfig>, IParamBusConfigManager
    {
        public ParamBusConfigManager(IParamBusConfigRepository repository) : base(repository)
        {
        }
    }
}

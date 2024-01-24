using BLL.Managers.Base;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class ParamBusConfigManager : Manager<ParamBusConfig>, IParamBusConfigManager
    {
        private readonly IParamBusConfigRepository _repository;

        public ParamBusConfigManager(IParamBusConfigRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ParamBusConfig> GetByParamSL(int paramSL)
        {
            return await _repository.Table.FirstOrDefaultAsync(s => s.ParamSL == paramSL);
        }
    }
}

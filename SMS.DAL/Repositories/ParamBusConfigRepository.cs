using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;

namespace SMS.DAL.Repositories
{
    public class ParamBusConfigRepository : Repository<ParamBusConfig>, IParamBusConfigRepository
    {
        public ParamBusConfigRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

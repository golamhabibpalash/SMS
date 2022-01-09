using SMS.BLL.Contracts.Base;
using SMS.Entities;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IInstituteManager:IManager<Institute>
    {
        Task<Institute> GetFirstOrDefaultAsync();
    }
}

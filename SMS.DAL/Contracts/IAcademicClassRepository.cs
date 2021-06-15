using SMS.DAL.Contracts.Base;
using SMS.Entities;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IAcademicClassRepository : IRepository<AcademicClass>
    {
        Task<AcademicClass> GetByNameAsync(string entityName);
    }
}

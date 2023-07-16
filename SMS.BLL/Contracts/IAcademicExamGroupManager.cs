using SMS.BLL.Contracts.Base;
using SMS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IAcademicExamGroupManager:IManager<AcademicExamGroup>
    {
        Task<IReadOnlyCollection<AcademicExamGroup>> GetAllAsync(int SessionId);
    }
}

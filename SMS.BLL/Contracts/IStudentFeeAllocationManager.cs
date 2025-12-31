using SMS.BLL.Contracts.Base;
using SMS.Entities;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IStudentFeeAllocationManager : IManager<StudentFeeAllocation>
    {
        Task<StudentFeeAllocation> GetStudentFeeAllocationByUniqueIdFeeHeadId(string uniqueId, int feeHeadId);
        Task<StudentFeeAllocation> GetStudentFeeAllocationByUniqueIdClassFeeId(string uniqueId, int classfeeId);
    }
}

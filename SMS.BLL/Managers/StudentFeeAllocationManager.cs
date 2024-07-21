using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class StudentFeeAllocationManager : Manager<StudentFeeAllocation>, IStudentFeeAllocationManager
    {
        private readonly IStudentFeeAllocationRepository _repository;
        public StudentFeeAllocationManager(IStudentFeeAllocationRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<StudentFeeAllocation> GetStudentFeeAllocationByUniqueIdFeeHeadId(string uniqueId, int feeHeadId)
        {
            return await _repository.GetStudentFeeAllocationByUniqueIdFeeHeadId(uniqueId, feeHeadId);
        }
    }
}

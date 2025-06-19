using BLL.Managers.Base;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class StudentFeeAllocationManager : Manager<StudentFeeAllocation>, IStudentFeeAllocationManager
    {
        private readonly IStudentFeeAllocationRepository _feeAllocationRepository;
        public StudentFeeAllocationManager(IStudentFeeAllocationRepository repository) : base(repository)
        {
            _feeAllocationRepository = repository;
        }

        public Task<StudentFeeAllocation> GetStudentFeeAllocationByUniqueIdClassFeeId(string uniqueId, int classfeeId)
        {
            return _feeAllocationRepository.GetStudentFeeAllocationByUniqueIdClassFeeId(uniqueId, classfeeId);
        }

        public async Task<StudentFeeAllocation> GetStudentFeeAllocationByUniqueIdFeeHeadId(string uniqueId, int feeHeadId)
        {
            return await _feeAllocationRepository.GetStudentFeeAllocationByUniqueIdFeeHeadId(uniqueId, feeHeadId);
        }
         
        public override async Task<IReadOnlyCollection<StudentFeeAllocation>> GetAllAsync()
        {
            return await _feeAllocationRepository.Table
                .Include(s => s.ClassFeeList)
                    .ThenInclude(m => m.AcademicSession)
                .Include(c => c.Student)
                    .ThenInclude(l => l.AcademicClass)
                .Include(c => c.StudentFeeHead)
                .ToListAsync();
        }
    }
}

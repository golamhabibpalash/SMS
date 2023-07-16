using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories;
using SMS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class AcademicExamGroupManager:Manager<AcademicExamGroup>,IAcademicExamGroupManager
    {
        IAcademicExamGroupRepository _academicExamGroupRepository;
        public AcademicExamGroupManager(IAcademicExamGroupRepository academicExamGroupRepository):base(academicExamGroupRepository)
        {
            _academicExamGroupRepository = academicExamGroupRepository;
        }
        public async Task<IReadOnlyCollection<AcademicExamGroup>> GetAllAsync(int SessionId)
        {
            var allExamGroup = await _academicExamGroupRepository.GetAllAsync();
            var result = allExamGroup.Where(s => s.AcademicSessionId ==SessionId).ToList();

            return result;
        }
    }
}

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
    public class AcademicExamGroupManager : Manager<AcademicExamGroup>, IAcademicExamGroupManager
    {
        IAcademicExamGroupRepository _academicExamGroupRepository;
        public AcademicExamGroupManager(IAcademicExamGroupRepository academicExamGroupRepository) : base(academicExamGroupRepository)
        {
            _academicExamGroupRepository = academicExamGroupRepository;
        }
        public override async Task<AcademicExamGroup> GetByIdAsync(int id)
        {
            var examGroup = await _academicExamGroupRepository
                .Table
                .Include(g => g.AcademicExams)
                    .ThenInclude(e => e.AcademicClass)
                        .ThenInclude(c => c.AcademicSections)
                .Include(g => g.AcademicSession)
                .Include(g => g.academicExamType)
                .FirstOrDefaultAsync(g => g.Id == 47);


            return examGroup;
        }
        public override async Task<IReadOnlyCollection<AcademicExamGroup>> GetAllAsync()
        {
            var result =await _academicExamGroupRepository
                .Table
                .Include(s => s.AcademicExams)
                    .ThenInclude(e => e.AcademicClass)
                .ToListAsync();
            return result;
        }
        public async Task<IReadOnlyCollection<AcademicExamGroup>> GetAllAsync(int SessionId)
        {
            var allExamGroup = await _academicExamGroupRepository.GetAllAsync();
            var result = allExamGroup.Where(s => s.AcademicSessionId == SessionId).ToList();

            return result;
        }

        public async Task<IReadOnlyCollection<AcademicExamGroup>> GetByMonthExamType(int monthId, int examTypeId)
        {
            var result = await _academicExamGroupRepository.GetByMonthExamType(monthId, examTypeId);
            return result;
        }
        public async Task<IReadOnlyCollection<AcademicExamGroup>> GetBySession(int sessionId)
        {
            var allExamGroup = await _academicExamGroupRepository.GetAllAsync();
            var examGroupBySession = allExamGroup.Where(e => e.AcademicSessionId.Equals(sessionId)).ToList();
            return examGroupBySession;
        }

    }
}

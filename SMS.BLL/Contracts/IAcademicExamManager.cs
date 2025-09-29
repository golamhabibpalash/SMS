using SMS.BLL.Contracts.Base;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts;

public interface IAcademicExamManager:IManager<AcademicExam>
{
    Task<List<AcademicExam>> GetByClassIdExamGroupId(int examGroupId, int academicClassId);
    Task<List<ExamSessionDto>> GetExaminationListAsync();
    Task<LiveResultVM> GetLiveResultByGroupIdClassId(int academicGroupId, int academiClassId);
}

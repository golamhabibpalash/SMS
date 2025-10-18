using SMS.BLL.Contracts.Base;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts;

public interface IAcademicExamManager:IManager<AcademicExam>
{
    Task<List<AcademicExam>> GetByClassIdExamGroupIdAsync(int examGroupId, int academicClassId);
    Task<List<ExamSessionDto>> GetExaminationListAsync();
    Task<LiveResultVM> GetLiveResultByGroupIdClassIdSectionId(int academicGroupId, int academiClassId, int? academicSectionId);
    Task<AcademicExam> GetAcademicExam(int examGroupId, int classId, int subjectId, string examType);
}

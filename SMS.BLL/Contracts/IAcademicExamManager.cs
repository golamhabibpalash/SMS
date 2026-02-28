using SMS.BLL.Contracts.Base;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using SMS.Entities.AdditionalModels.StudentVM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts;

public interface IAcademicExamManager:IManager<AcademicExam>
{
    Task<List<AcademicExam>> GetByClassIdExamGroupIdAsync(int examGroupId, int academicClassId);
    Task<List<ExamSessionDto>> GetExaminationListAsync();
    Task<LiveResultVM> GetLiveResultByGroupIdClassIdSectionId(int academicGroupId, int academiClassId, int? academicSectionId);
    Task<AcademicExam> GetAcademicExam(int examGroupId, int classId, int subjectId, string examType, int? sectionId);
    Task<int> GetTotalExamAsync(int examGroupId, int classId);
    Task<List<AcademicClass>> GetAcademicClassListByGroupIdAsync(int groupId);
    Task<ProfileResult> GetSingleStudentResultDetailForProfile(int studentId);
}

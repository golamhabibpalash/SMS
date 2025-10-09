using BLL.Managers.Base;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class AcademicExamManager:Manager<AcademicExam>, IAcademicExamManager
    {
        private readonly IAcademicExamRepository _academicExamRepository;
        private readonly IAcademicSessionRepository _academicSessionRepository;
        private readonly IAcademicExamGroupRepository _academicExamGroupRepository;
        private readonly IAcademicClassRepository _academicClassRepository;
        private readonly IGradingTableRepository _gradingTableRepository;
        private AcademicExamGroup _cachedExamGroup;
        private List<GradingTable> _cachedGradingTable;
        public AcademicExamManager(IAcademicExamRepository academicExamRepository, IAcademicSessionRepository academicSessionRepository, IAcademicExamGroupRepository academicExamGroupRepository, IAcademicClassRepository academicClassRepository, IGradingTableRepository gradingTableRepository) : base(academicExamRepository)
        {
            _academicExamRepository = academicExamRepository;
            _academicSessionRepository = academicSessionRepository;
            _academicExamGroupRepository = academicExamGroupRepository;
            _academicClassRepository = academicClassRepository;
            _gradingTableRepository = gradingTableRepository;
        }

        public async Task<List<AcademicExam>> GetByClassIdExamGroupId(int examGroupId, int academicClassId)
        {
            var result = await _academicExamRepository.GetByClassIdExamGroupId(examGroupId, academicClassId);
            return result;
        }
        public async Task<List<ExamSessionDto>> GetExaminationListAsync()
        {
            var allSessions = await _academicSessionRepository.GetAllAsync();
            var allExaminations = await _academicExamRepository.GetAllAsync();

            var result = new List<ExamSessionDto>();

            foreach (var session in allSessions)
            {
                var sessionDto = new ExamSessionDto
                {
                    Id = session.Id,
                    SessionName = session.Name,
                    ExamGroupDtos = new List<ExamGroupDto>()
                };

                // Group exams by ExamGroupId
                var examsForSession = allExaminations
                    .Where(e => e.AcademicExamGroup.AcademicSessionId == session.Id)
                    .GroupBy(e => e.AcademicExamGroupId);

                foreach (var examGroup in examsForSession)
                {
                    var targetedExams = allExaminations.Where(s => s.AcademicExamGroupId == examGroup.Key);
                    ExamGroupDto examGroupDto = new ExamGroupDto();
                    examGroupDto.Id = examGroup.Key;
                    examGroupDto.Name = examGroup.FirstOrDefault().AcademicExamGroup.ExamGroupName;
                    examGroupDto.ExaminationDtos = targetedExams.Select(s => MapToExaminationDto(s)).ToList();
                    sessionDto.ExamGroupDtos.Add(examGroupDto);
                }

                result.Add(sessionDto);
            }

            return result;
        }
        public async Task<LiveResultVM> GetLiveResultByGroupIdClassId(int academicGroupId, int academiClassId)
        {
            var liveResultVM = new LiveResultVM();

            var examGroup = await _academicExamGroupRepository.GetByIdAsync(academicGroupId);
            examGroup.AcademicExams = examGroup.AcademicExams.Where(s => s.AcademicClassId == academiClassId).ToList();
            if (examGroup==null)
            {
                return liveResultVM;
            }
            _cachedGradingTable = (List<GradingTable>)await _gradingTableRepository.GetAllAsync();
            _cachedExamGroup = examGroup;
            var existingExams = examGroup.AcademicExams.Where(s => s.AcademicClassId == academiClassId).ToList();
            if (existingExams==null || existingExams.Count<=0)
            {
                return liveResultVM;
            }
            var academicClass = await _academicClassRepository.GetByIdAsync(academiClassId);

            var existingStudents = existingExams.SelectMany(s => s.AcademicExamDetails).Select(s => s.Student).DistinctBy(s => s.Id).ToList();

            if (existingStudents!=null)
            {
                foreach (var student in existingStudents)
                {
                    LiveResultDetailsVM liveResultDetailsVM = new LiveResultDetailsVM
                    {
                        ClassRoll = student.ClassRoll.ToString(),
                        StudentName = student.Name,
                        FinalGPA = GetFinalGPA(student.Id, academicGroupId),
                        FinalGrade = GetFinalGrade(student.Id,academicGroupId),
                        TotalMarks = GetTotalMarks(student.Id),
                        Attendance = GetAttendance(student.Id, academicGroupId),
                        Rank = GetFinalRank(student.Id, academicGroupId),
                        LiveResultSubjectWises = GetLiveResultSubjectWise(student.Id)
                    };
                    liveResultVM.ResultDetails.Add(liveResultDetailsVM);
                }
            }

            if (existingExams != null || existingExams.Count > 0)
            {
                liveResultVM.ExamTitle = $"{examGroup.ExamGroupName} ({academicClass.Name})";
                liveResultVM.TotalColumn = 7+liveResultVM.ResultDetails.Count;
                liveResultVM.Subjects = GetTableHeaderSubjects(existingExams);
            }

            return liveResultVM;
        }

        private double GetFinalGPA(int id, int academicGroupId)
        {
            var finalGPA = 6.00;
            return finalGPA;
        }

        private int GetFinalRank(int id, int academicGroupId)
        {
            var result = 3;
            return result;
        }

        private double GetAttendance(int id, int academicGroupId)
        {
            var result = 87.44;
            return result;
        }

        private string GetFinalGrade(int id, int academicGroupId)
        {
            var result = "ABC+";
            return result;
        }

        private double GetTotalMarks(int id)
        {
            var result = 0.0;
            foreach (var exam in _cachedExamGroup.AcademicExams)
            {
                var examDetail = exam.AcademicExamDetails.FirstOrDefault(s => s.StudentId == id);
                if (examDetail != null)
                {
                    result += examDetail.ObtainMark;
                }
            }
            return result;
        }

        private List<TableHeaderSubjects> GetTableHeaderSubjects(List<AcademicExam> existingExams)
        {
            var subjectList = new List<TableHeaderSubjects>();
            foreach (var item in existingExams)
            {
                bool isExist = subjectList.Any(s => s.SubjectName == item.AcademicSubject.SubjectName);
                if (isExist)
                {
                    continue;
                }
                TableHeaderSubjects headerSubjects = new TableHeaderSubjects()
                {
                    SubjectName = item.AcademicSubject.SubjectName,
                    TotalMarks = item.TotalMarks,
                    ExamTypes = GetExamTypes(existingExams,item.AcademicSubjectId),
                };
                subjectList.Add(headerSubjects);
            }
            return subjectList;
        }

        private List<TableHeaderExamTypes> GetExamTypes(List<AcademicExam> existingExams, int subjectId)
        {
            var types = new List<TableHeaderExamTypes>();
            var targetedExams = existingExams.Where(s => s.AcademicSubjectId == subjectId);
            foreach (var item in targetedExams)
            {
                TableHeaderExamTypes examType = new TableHeaderExamTypes()
                {
                    ExamType = item.ExamCategory??"CQ",
                    TotalMarks = item.TotalMarks
                };
                types.Add(examType);
            }
            return types;
        }

        private List<LiveResultSubjectWise> GetLiveResultSubjectWise(int studentId)
        {
            var liveResultSubjectWise = new List<LiveResultSubjectWise>();
            if (_cachedExamGroup.AcademicExams.Count>0)
            {
                var examList = _cachedExamGroup.AcademicExams.GroupBy(s => new { s.AcademicSubjectId, s.AcademicSubject.SubjectName, s.TotalMarks}).ToList();

                foreach (var exam in examList)
                {
                    var subWiseObtainMark = GetSubjectWiseTotalMarks(exam.Key.AcademicSubjectId, studentId);
                    LiveResultSubjectWise lrsw = new()
                    {
                        SubjectName = exam.Key.SubjectName,
                        Marks = exam.Key.TotalMarks,
                        GPA = GetGPAForSingleSubject(subWiseObtainMark,exam.Key.TotalMarks),
                        SubjectTypes = GetLiveResultSubjectType(exam.Key.AcademicSubjectId, studentId),
                        ObtainMarks = subWiseObtainMark
                    };
                    lrsw.TotalColumn = 2 + lrsw.SubjectTypes.Count;
                    var isExistOnSubjectWise = liveResultSubjectWise.FirstOrDefault(s => s.SubjectName == lrsw.SubjectName);
                    if (isExistOnSubjectWise == null)
                    {
                        liveResultSubjectWise.Add(lrsw);
                    }
                }
            }
            return liveResultSubjectWise;
        }

        private double GetGPAForSingleSubject(double obtainMark,int totalMarks)
        {
            //Make 100%
            var hundredPercentMark = (obtainMark * 100) / totalMarks;

            //Compare and Calculation Grade Point
            
            var gradePoint = _cachedGradingTable
                .FirstOrDefault(g => hundredPercentMark >= g.NumberRangeMin && hundredPercentMark <= g.NumberRangeMax)?
                .GradePoint ?? 0m; // or any default value


            return (double)gradePoint;
        }

        private double GetSubjectWiseTotalMarks(int examId, int studentId)
        {
            var result = 0.0;
            result = _cachedExamGroup.AcademicExams.SelectMany(s => s.AcademicExamDetails.Where(e => e.AcademicExam.AcademicSubjectId == examId && e.StudentId == studentId)).Sum(c => c.ObtainMark);
            
            return result;
        }

        private List<LiveResultSubjectType> GetLiveResultSubjectType(int academicSubjectId, int studentId)
        {
            var liveResultSubjectTypes = new List<LiveResultSubjectType>();
            var existingExams = _cachedExamGroup.AcademicExams.Where(s => s.AcademicSubjectId == academicSubjectId).ToList();
            foreach (var item in existingExams)
            {
                LiveResultSubjectType liveResultSubjectType = new LiveResultSubjectType()
                {
                    SubjectTypeName = item.ExamCategory,
                    TotalMarks =item.TotalMarks,
                    GetMarks = item.AcademicExamDetails.FirstOrDefault(s => s.StudentId == studentId).ObtainMark
                };
                var isExist = liveResultSubjectTypes.Any(s => s.SubjectTypeName == liveResultSubjectType.SubjectTypeName);
                if (!isExist)
                {
                    liveResultSubjectTypes.Add(liveResultSubjectType);
                }
            }
            return liveResultSubjectTypes;
        }

        private ExaminationDto MapToExaminationDto(AcademicExam exam)
        {
            return new ExaminationDto
            {
                Id = exam.Id,
                ClassName = exam.AcademicClass.Name,
                ClassId = exam.AcademicClassId,
                ExamGroupVMId = exam.AcademicExamGroupId,
                ExamCategory = exam.ExamCategory,
                // Fix: prevent duplicate details
                ExaminationDetailsDtos = exam.AcademicExamDetails
                    .GroupBy(d => new { exam.AcademicSubjectId, exam.AcademicSectionId, exam.EmployeeId})
                    .Select(g => new ExaminationDetailsDto
                    {
                        Id = g.First().Id,
                        SubjectName = exam.AcademicSubject.SubjectName,
                        SubjectId = exam.AcademicSubjectId,
                        SectionName = exam.AcademicSection?.Name,
                        SectionId = exam.AcademicSection?.Id,
                        EmployeeName = exam.Employee.EmployeeName,
                        EmployeeId = exam.EmployeeId,
                        TotalStudents = g.Count(),
                        TotalMarks = exam.TotalMarks,
                        IsLocked = g.First().Status
                    }).ToList()
            };
        }

        public async Task<AcademicExam> GetAcademicExam(int examGroupId, int classId, int subjectId, string examCategory)
        {
            var existingExam =await _repository.Table.FirstOrDefaultAsync(s => s.AcademicExamGroupId == examGroupId && s.AcademicClassId == classId && s.AcademicSubjectId == subjectId && s.ExamCategory == examCategory);
            return existingExam;
        }
    }
}

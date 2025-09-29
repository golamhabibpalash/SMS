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
        private readonly IStudentRepository _studentRepository;
        public AcademicExamManager(IAcademicExamRepository academicExamRepository, IAcademicSessionRepository academicSessionRepository, IAcademicExamGroupRepository academicExamGroupRepository, IAcademicClassRepository academicClassRepository, IStudentRepository studentRepository) : base(academicExamRepository)
        {
            _academicExamRepository = academicExamRepository;
            _academicSessionRepository = academicSessionRepository;
            _academicExamGroupRepository = academicExamGroupRepository;
            _academicClassRepository = academicClassRepository;
            _studentRepository = studentRepository;
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

            var existingExams = await _academicExamRepository.GetByClassIdExamGroupId(academicGroupId, academiClassId);
            var examGroup = await _academicExamGroupRepository.GetByIdAsync(academicGroupId);
            var academicClass = await _academicClassRepository.GetByIdAsync(academiClassId);
            var existingStudents = await _studentRepository.GetStudentsByClassIdAndSessionIdAsync(examGroup.AcademicSessionId, academiClassId);
            
            if (existingStudents!=null)
            {
                foreach (var student in existingStudents)
                {
                    LiveResultDetailsVM liveResultDetailsVM = new LiveResultDetailsVM
                    {
                        ClassRoll = student.ClassRoll.ToString(),
                        StudentName = student.Name,
                        FinalGPA = 0,
                        FinalGrade = "A+",
                        TotalMarks = 2000,
                        Attendance = 100,
                        Rank = 0,
                        LiveResultSubjectWises = GetLiveResultSubjectWise(existingExams)
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

        private List<LiveResultSubjectWise> GetLiveResultSubjectWise(List<AcademicExam> existingExams)
        {
            var liveResultSubjectWise = new List<LiveResultSubjectWise>();
            if (existingExams!=null)
            {
                foreach (var sub in existingExams)
                {
                    LiveResultSubjectWise lrsw = new LiveResultSubjectWise()
                    {

                        SubjectName = sub.AcademicSubject.SubjectName,
                        GPA = 4.00,
                        Marks = 20,
                        SubjectTypes = GetLiveResultSubjectType(existingExams,sub.AcademicSubjectId)
                    };
                    lrsw.TotalColumn = 2 + lrsw.SubjectTypes.Count;
                    liveResultSubjectWise.Add(lrsw);
                }
            }
            return liveResultSubjectWise;
        }

        private List<LiveResultSubjectType> GetLiveResultSubjectType(List<AcademicExam> existingExams, int academicSubjectId)
        {
            var liveResultSubjectTypes = new List<LiveResultSubjectType>();
            var examCategories = existingExams.GroupBy(s => new {s.AcademicExamGroupId,s.AcademicClassId,s.AcademicSubjectId }).ToList();
            foreach (var item in examCategories.Where(s => s.Key.AcademicSubjectId == academicSubjectId))
            {
                var subCats = existingExams.Where(s => s.AcademicClassId == item.Key.AcademicClassId && s.AcademicExamGroupId == item.Key.AcademicExamGroupId && item.Key.AcademicSubjectId == s.AcademicSubjectId);
                foreach (var cat in subCats)
                {
                    LiveResultSubjectType liveResultSubjectType = new LiveResultSubjectType()
                    {
                        SubjectTypeName = cat.ExamCategory+$"({cat.TotalMarks})"
                    };
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


    }
}

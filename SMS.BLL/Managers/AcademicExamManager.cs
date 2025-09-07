using BLL.Managers.Base;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
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
        public AcademicExamManager(IAcademicExamRepository academicExamRepository, IAcademicSessionRepository academicSessionRepository, IAcademicExamGroupRepository academicExamGroupRepository) : base(academicExamRepository)
        {
            _academicExamRepository = academicExamRepository;
            _academicSessionRepository = academicSessionRepository;
            _academicExamGroupRepository = academicExamGroupRepository;
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

        private ExaminationDto MapToExaminationDto(AcademicExam exam)
        {
            return new ExaminationDto
            {
                Id = exam.Id,
                ClassName = exam.AcademicClass.Name,
                ClassId = exam.AcademicClassId,
                ExamGroupVMId = exam.AcademicExamGroupId,

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

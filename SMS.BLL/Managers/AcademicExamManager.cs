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
                    var firstExam = examGroup.First();

                    var examGroupDto = new ExamGroupDto
                    {
                        Id = firstExam.AcademicExamGroup.Id,
                        Name = firstExam.AcademicExamGroup.ExamGroupName,
                        ExamSessionVMId = firstExam.AcademicExamGroup.AcademicSessionId,
                        ExaminationDtos = new List<ExaminationDto>()
                    };

                    foreach (var exam in examGroup)
                    {
                        // Check if exam already exists in the group
                        var existingExamDto = examGroupDto.ExaminationDtos
                            .FirstOrDefault(e => e.ExamGroupVMId == exam.AcademicExamGroupId);

                        if (existingExamDto != null)
                        {
                            // Only add details (prevent duplicate exam)
                            var newDetails = exam.AcademicExamDetails
                                .GroupBy(d => new { exam.AcademicSubjectId, exam.AcademicSectionId, exam.EmployeeId })
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
                                });

                            // Add only if not already present
                            foreach (var detail in newDetails)
                            {
                                if (!existingExamDto.ExaminationDetailsDtos.Any(d => d.Id == detail.Id))
                                {
                                    existingExamDto.ExaminationDetailsDtos.Add(detail);
                                }
                            }
                        }
                        else
                        {
                            // Create a new ExaminationDto
                            var examinationDto = MapToExaminationDto(exam);
                            examGroupDto.ExaminationDtos.Add(examinationDto);
                        }
                    }

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

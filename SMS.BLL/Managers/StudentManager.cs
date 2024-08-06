using BLL.Managers.Base;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.BLL.Contracts.Base;
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
    public class StudentManager :Manager<Student>, IStudentManager
    {
        private readonly IStudentRepository studentRepository;

        public StudentManager(IStudentRepository studentRepository):base(studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        public async Task<List<StudentListVM>> GetCurrentStudentListAsync(int? AcademicClassId, int? AcademicSectionId)
        {
            return await studentRepository.GetCurrentStudentListAsync(AcademicClassId, AcademicSectionId);
        }

        public async Task<Student> GetStudentByClassRollAsync(int classRoll)
        {
            return await studentRepository.GetStudentByClassRollAsync(classRoll);
        }
        public async Task<Student> GetStudentByUniqueIdAsync(string uniqueId)
        {
            return await studentRepository.GetStudentByUniqueIdAsync(uniqueId);
        }

        public async Task<Student> GetStudentByClassRollAsync(int id, int classRoll)
        {
            return await studentRepository.GetStudentByClassRollAsync(id, classRoll);
        }

        public async Task<List<Student>> GetStudentsByClassIdAndSessionIdAsync(int sessionId, int classId)
        {
            return await studentRepository.GetStudentsByClassIdAndSessionIdAsync(sessionId, classId);
        }

        public async Task<List<Student>> GetStudentsByClassSessionSectionAsync(int sessionId, int classId, int sectionId)
        {
            return await studentRepository.GetStudentsByClassSessionSectionAsync(sessionId, classId, sectionId);
        }
        public async Task<string> GetUniqueIdByStudentId(int stuId)
        {
            Student student = await studentRepository.GetByIdAsync(stuId);
            return student.UniqueId;
        }

        public async Task<List<StudentListVM>> GetStudentsBySearch(string search)
        {
            search = search.Trim().ToLower();
            var students = await studentRepository.GetAllAsync();

            students = students
                .Where(s =>
                    (s.Name?.ToLower().Contains(search) ?? false) ||
                    (s.NameBangla?.Contains(search) ?? false) ||
                    (s.UniqueId?.Contains(search) ?? false) ||
                    (s.AcademicClass?.Name?.ToLower().Contains(search) ?? false) ||
                    (s.ClassRoll.ToString().Contains(search)) ||
                    (s.AcademicSection?.Name?.ToLower().Contains(search) ?? false) ||
                    (s.PhoneNo?.Contains(search) ?? false))
                .ToList();


            List<StudentListVM> studentListVMs = new();

            studentListVMs= students.Select(student => new StudentListVM
            {
                Id = student.Id,
                ClassRoll = student.ClassRoll,
                Photo = student.Photo,
                StudentName = student.Name,
                NameBangla = student.NameBangla,
                ClassName = student.AcademicClass?.Name,
                SectionName = student.AcademicSection?.Name,
                PhoneNo = student.PhoneNo,
                SessionName = student.AcademicSession?.Name,
                Gender = student.Gender?.Name,
                Status = student.Status,
                ClassSerial = student.AcademicClass?.ClassSerial,
                IsResidential = student.IsResidential,
                UniqueId = student.UniqueId,
            }).ToList();
            return studentListVMs;
        }
    }
}

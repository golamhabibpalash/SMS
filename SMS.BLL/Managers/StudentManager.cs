using BLL.Managers.Base;
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
        public async Task<string> GetUniqueIdByStudenyId(int stuId)
        {
            Student student = await studentRepository.GetByIdAsync(stuId);
            return student.UniqueId;
        }
    }
}

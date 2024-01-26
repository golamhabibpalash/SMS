using SMS.DAL.Contracts.Base;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student> GetStudentByClassRollAsync(int classRoll);
        Task<Student> GetStudentByUniqueIdAsync(string uniqueId);
        Task<Student> GetStudentByClassRollAsync(int id, int classRoll);
        Task<List<Student>> GetStudentsByClassIdAndSessionIdAsync(int sessionId, int classId);
        Task<List<Student>> GetStudentsByClassSessionSectionAsync(int sessionId, int classId, int sectionId);
        Task<List<StudentListVM>> GetCurrentStudentListAsync(int? AcademicClassId, int? AcademicSectionId);
        Task<Student> GetStudentByUniqueIdAsync(string uniqueId);
    }
}

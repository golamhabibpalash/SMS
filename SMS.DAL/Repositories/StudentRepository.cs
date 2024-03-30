using SMS.DB;
using SMS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SMS.DAL.Repositories.Base;
using SMS.DAL.Contracts;
using System.Linq;
using Microsoft.Data.SqlClient;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SMS.Entities.AdditionalModels;
using SMS.Entities.RptModels;

namespace SMS.DAL.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {

        public StudentRepository(ApplicationDbContext context) : base(context)
        {

        }

        public override async Task<IReadOnlyCollection<Student>> GetAllAsync()
        {
            return await _context.Student
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicSession)
                .Include(s => s.AcademicSection)
                .Include(s => s.Gender)
                .OrderBy(s => s.AcademicClassId)
                .ThenBy(s => s.ClassRoll)
                .ToListAsync();
        }
        public override async Task<Student> GetByIdAsync(int id)
        {
            Student result = await _context.Student.Include(s => s.AcademicClass)
                .Include(s => s.AcademicSection)
                .Include(s => s.AcademicSession)
                .Include(s => s.BloodGroup)
                .Include(s => s.Gender)
                .Include(s => s.Nationality)
                .Include(s => s.Religion)
                .Include(s => s.PresentUpazila)
                .Include(s => s.PresentDistrict)
                .Include(s => s.PresentDivision)
                .Include(s => s.PermanentUpazila)
                .Include(s => s.PermanentDistrict)
                .Include(s => s.PermanentDivision)
                .FirstOrDefaultAsync(s => s.Id == id);

            return result;
        }

        public async Task<List<StudentListVM>> GetCurrentStudentListAsync(int? AcademicClassId, int? AcademicSectionId)
        {
            var pAcademicClassId = new SqlParameter("@academicClassId", (object)AcademicClassId ?? DBNull.Value);
            var pAcademicSectionId = new SqlParameter("@academicSectionId", (object)AcademicSectionId ?? DBNull.Value);
            List<StudentListVM> studentListVMs = null;
            try
            {
                studentListVMs = await _context.StudentListVMs
                    .FromSqlInterpolated($"EXECUTE sp_Get_Current_Student_List {pAcademicClassId}, {pAcademicSectionId}")
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return studentListVMs;
        }

        public async Task<Student> GetStudentByClassRollAsync(int classRoll)
        {
            var student = await _context.Student
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicSession)
                .Include(s => s.AcademicSection)
                .FirstOrDefaultAsync(s => s.ClassRoll == classRoll);
            return student;
        }
        public async Task<Student> GetStudentByUniqueIdAsync(string uniqueId)
        {
            var student = await _context.Student
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicSession)
                .Include(s => s.AcademicSection)
                .FirstOrDefaultAsync(s => Convert.ToInt32(s.UniqueId.Trim()).ToString() == uniqueId.Trim());
            return student;
        }

        public async Task<Student> GetStudentByClassRollAsync(int id, int classRoll)
        {
            return await _context.Student
                .FirstOrDefaultAsync(s => s.Id != id 
                && s.ClassRoll == classRoll);
        }

        public async Task<List<Student>> GetStudentsByClassIdAndSessionIdAsync(int sessionId, int classId)
        {
            List<Student> students = await _context.Student
                .Include(s => s.AcademicSection)
                .Include(s => s.Gender)
                .Where(s => s.AcademicSessionId == sessionId 
                && s.AcademicClassId == classId)
                .ToListAsync();
            return students;
        }

        public async Task<List<Student>> GetStudentsByClassSessionSectionAsync(int sessionId, int classId, int sectionId)
        {
            List<Student> students = new List<Student>();
            students = await _context
                .Student
                .Include(s => s.AcademicSection)
                .Include(s => s.AcademicSession)
                .Include(s => s.AcademicClass)
                .ToListAsync();

            if (classId!=0)
            {
                students = students.Where(s => s.AcademicClassId == classId).ToList();
            }

            if (sectionId != 0)
            {
                students = (List<Student>)students.Where(s => s.AcademicSectionId == sectionId).ToList();
            }
            if (sessionId != 0)
            {
                students = (List<Student>)students.Where(s => s.AcademicSessionId == sessionId).ToList();
            }
            return students;
        }
    }
}

using SMS.DB;
using SMS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SMS.DAL.Repositories.Base;
using SMS.DAL.Contracts;
using System.Linq;

namespace SMS.DAL.Repositories
{
    public class StudentRepository : Repository<Student>,IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context):base(context)
        {
        }
        public override async Task<IReadOnlyCollection<Student>> GetAllAsync()
        {
            return await _context.Student.Include(s => s.AcademicClass)
                .Include(s => s.AcademicSession).ToListAsync();
        }
        public override async Task<Student> GetByIdAsync(int id)
        {
            return await _context.Student.Include(s => s.AcademicClass)
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

        public async Task<Student> GetStudentByClassRollAsync(int id, int classRoll)
        {
            return await _context.Student.FirstOrDefaultAsync(s => s.Id != id && s.ClassRoll == classRoll);
        }

        public async Task<List<Student>> GetStudentsByClassIdAndSessionIdAsync(int sessionId, int classId)
        {
            List<Student> students = await _context.Student.Where(s => s.AcademicSessionId == sessionId && s.AcademicClassId == classId).ToListAsync();
            return students;
        }
    }
}

using SMS.DB;
using SMS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SMS.DAL.Repositories.Base;
using SMS.DAL.Contracts;

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

    }
}

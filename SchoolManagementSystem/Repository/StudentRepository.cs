using Microsoft.EntityFrameworkCore;
using DatabaseContext;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Repository
{
    public class StudentRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public Student GetById(int id)
        {
            var result = _context.Student.FirstOrDefault(g => g.Id == id);
            return result;
        }
        public async Task<Student> GetByIdAsync(int id)
        {
            var student =await _context.Student
                .Include(s => s.AcademicSession)
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicSection)
                .Include(s => s.Nationality)
                .Include(s => s.Gender)
                .Include(s => s.Religion)
                .Include(s => s.PresentDistrict)
                .Include(s => s.PermanentDistrict)
                .Include(s => s.PresentUpazila)
                .Include(s => s.PermanentUpazila)
                .Include(s => s.PresentDivision)
                .Include(s => s.PermanentDivision)
                .Include(s => s.BloodGroup)
                .FirstOrDefaultAsync(s => s.Id == id);
            return student;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            var result = await _context
                .Student
                .Include(s => s.AcademicSession)
                .Include(s => s.AcademicClass)
                .OrderBy(s => s.AcademicSessionId)
                .ThenBy(s => s.ClassRoll)
                .ToListAsync();

            return result;
        }

        public async Task<bool> SaveAsync(Student student)
        {
            _context.Student.Add(student);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            _context.Student.Attach(student);
            _context.Entry(student).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAsync(Student student)
        {
            _context.Student.Remove(student);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

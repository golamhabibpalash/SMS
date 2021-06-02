using Microsoft.EntityFrameworkCore;
using DatabaseContext;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Repository
{
    public class AcademicClassRepository
    {
        private readonly ApplicationDbContext _context;
        public AcademicClassRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public AcademicClass GetById(int id)
        {
            var academicClass = _context.AcademicClass.FirstOrDefault(s => s.Id == id);
            return academicClass;
        }
        public List<AcademicClass> GetAll()
        {
            var result = _context.AcademicClass
                .OrderBy(c => c.ClassSerial)
                .ToList();

            return result;
        }
        public async Task<List<AcademicClass>> GetAllAsync()
        {
            var result = await _context.AcademicClass.ToListAsync();
            return result;
        }
        public async Task<List<AcademicClass>> GetAllBySessionIdAsync(int id)
        {
            var result = await _context.AcademicClass
                .OrderBy(a => a.ClassSerial)
                .ToListAsync();

            return result;
        }

        public async Task<AcademicClass> GetByIdAsync(int id)
        {
            var academicClass = await _context.AcademicClass.FirstOrDefaultAsync(s => s.Id == id);
            return academicClass;
        }

        public async Task<bool> SaveAsync(AcademicClass academicClass)
        {
            _context.AcademicClass.Add(academicClass);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(AcademicClass academicClass)
        {
            _context.AcademicClass.Attach(academicClass);
            _context.Entry(academicClass).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAsync(AcademicClass academicClass)
        {
            _context.AcademicClass.Remove(academicClass);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

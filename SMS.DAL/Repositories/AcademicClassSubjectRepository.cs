using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class AcademicClassSubjectRepository:Repository<AcademicClassSubject>, IAcademicClassSubjectRepository
    {
        //private readonly ApplicationDbContext _context;
        public AcademicClassSubjectRepository(ApplicationDbContext context):base(context)
        {
            //_context = context;
        }
        public override async Task<IReadOnlyCollection<AcademicClassSubject>> GetAllAsync()
        {
            var result = await _context
                .AcademicClassSubjects
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicSubject)
                    .ThenInclude(s => s.AcademicSubjectType)
                .ToListAsync();
            return result;
        }

        public async Task<List<AcademicSubject>> GetSubjectsByClassIdAsync(int classId)
        {
            if (classId<=0)
            {
                return new List<AcademicSubject>();
            }
            var academicSubjects = await _context.AcademicClassSubjects
                .Include(s => s.AcademicSubject)
                .ThenInclude(s => s.AcademicSubjectType)
                .Where(s => s.AcademicClassId==classId)
                .Select(s => s.AcademicSubject)
                .ToListAsync();

            return academicSubjects; 
        }
    }
}

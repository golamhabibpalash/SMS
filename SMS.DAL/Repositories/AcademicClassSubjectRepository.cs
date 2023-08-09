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
            var result = await _context.AcademicClassSubjects.Include(s => s.AcademicClass).Include(s => s.AcademicSubject).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<AcademicSubject>> GetSubjectsByClassIdAsync(int classId)
        {
            var subjects = await _context.AcademicClassSubjects.Select(s=> s.AcademicSubject).Where(s => s.AcademicClassId == classId).ToListAsync();
            return subjects; 
        }
    }
}

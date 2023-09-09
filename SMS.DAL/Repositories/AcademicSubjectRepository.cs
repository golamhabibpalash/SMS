using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Contracts.Base;
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
    public class AcademicSubjectRepository:Repository<AcademicSubject>, IAcademicSubjectRepository
    {
        public AcademicSubjectRepository(ApplicationDbContext db):base(db)
        {

        }
        public override async Task<IReadOnlyCollection<AcademicSubject>> GetAllAsync()
        {
            return await _context.AcademicSubject
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicSubjectType)
                .OrderBy(s => s.SubjectCode)
                .ToListAsync();
        }
        public override async Task<bool> AddAsync(AcademicSubject entity)
        {
            await _context.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public override async Task<bool> IsExistAsync(AcademicSubject entity)
        {
            //var existSubject = await _context.AcademicSubject
            //    .Where(s => (s.SubjectName.Trim() == entity.SubjectName.Trim() || s.SubjectCode == entity.SubjectCode) && s.SubjectFor.ToString().Trim() == entity.SubjectFor.ToString().Trim())
            //    .FirstOrDefaultAsync();

           var existSubject =  await _context.AcademicSubject.Where(s => s.SubjectName.Trim() == entity.SubjectName.Trim()).FirstOrDefaultAsync();
            if (existSubject != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public async Task<IEnumerable<AcademicSubject>> GetSubjectsByClassIdAsync(int classId)
        //{
        //    var subjects = await _context.AcademicClassSubjects
        //        .Where(s => s.AcademicClassId==classId)
        //        .Select(s => s.AcademicSubject)
        //        .Include(s => s.AcademicClass)
        //        .ToListAsync();
        //    return subjects;
        //}

        public override async Task<AcademicSubject> GetByIdAsync(int id)
        {
            var existingSubject = await _context.AcademicSubject
                .Include(a => a.QuestionFormat)
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();

            return existingSubject;
        }

        public async Task<List<AcademicSubject>> GetSubjectsByClassIdAsync(int classId)
        {
            List<AcademicSubject> subObjects = new List<AcademicSubject>();
            List<AcademicSubject> subjects = await _context.AcademicSubject.Include(s => s.AcademicSubjectType).ToListAsync();
            List<AcademicClassSubject> classSubjects = await _context.AcademicClassSubjects.Where(s => s.AcademicClassId == classId).ToListAsync();
            foreach (var item in classSubjects)
            {
                foreach (var sub in subjects)
                {
                    if (item.AcademicSubjectId == sub.Id)
                    {
                        subObjects.Add(sub);
                    }
                }
            }
            return subObjects;
        }
    }
}

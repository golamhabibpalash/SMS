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

namespace SMS.DAL.Repositories
{
    public class StudentRepository : Repository<Student>,IStudentRepository
    {
        
        public StudentRepository(ApplicationDbContext context ) : base(context)
        {
           
        }
        
        public override async Task<IReadOnlyCollection<Student>> GetAllAsync()
        {
            return await _context.Student.Include(s => s.AcademicClass)
                .Include(s => s.AcademicSession).OrderBy(s => s.AcademicClassId).ThenBy(s => s.ClassRoll).ToListAsync();
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

        //public override async Task<bool> UpdateAsync(Student entity)
        //{
        //    Student existingStudenet = await GetByIdAsync(entity.Id); 
        //    List<StudentActivateHist> previousHist= await GetExistingHistory(entity.Id, entity.EditedAt.ToString("yyyy-MM-dd"));

        //    DateTime qDate = entity.AdmissionDate;
        //    if (existingStudenet.AdmissionDate.Date < qDate.Date)
        //    {
        //        var listOfActivationHistory = await GetExistingHistory(entity.Id, entity.EditedAt.Date.ToString("yyyy-MM-dd"));
        //        StudentActivateHist objStudentActivateHist = (from t in listOfActivationHistory
        //                                                      where t.ActionDateTime.Date < qDate.Date
        //                                                      select t).Last();
        //        return objStudentActivateHist.IsActive;
        //    }
        //    else
        //    {
        //        return false;
        //    }
            

        //    _context.Entry(entity).State = EntityState.Modified;
        //    bool isUpdated = await _context.SaveChangesAsync() > 0;
        //    if (isUpdated)
        //    {
        //        bool isChangeActiveStatus = true;

        //    }

        //    return isUpdated;
        //}

    }
}

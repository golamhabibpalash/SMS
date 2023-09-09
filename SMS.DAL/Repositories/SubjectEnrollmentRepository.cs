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
    public class SubjectEnrollmentRepository:Repository<SubjectEnrollment>, ISubjectEnrollmentRepository
    {
        public SubjectEnrollmentRepository(ApplicationDbContext context):base(context)
        {
            
        }

        public async Task<List<SubjectEnrollmentDetail>> GetAllDetailsByStudentIdAsync(int studentId)
        {
            List<SubjectEnrollmentDetail> result = new List<SubjectEnrollmentDetail>();
            try
            {
                result = await _context
                    .SubjectEnrollmentDetails
                    .Include(s => s.AcademicSubject)
                    .Include(s => s.AcademicSubjectType)
                    .Where(s => s.SubjectEnrollment.StudentId == studentId)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<SubjectEnrollment> GetByStudentIdAsync(int studentId)
        {
            SubjectEnrollment subjectEnrollment = await _context
                .SubjectEnrollments
                .Include(s => s.EnrolledSubjects)
                .SingleOrDefaultAsync(s => s.StudentId==studentId);
            return subjectEnrollment;
        }
    }
}

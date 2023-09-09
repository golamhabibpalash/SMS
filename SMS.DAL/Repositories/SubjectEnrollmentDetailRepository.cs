using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;

namespace SMS.DAL.Repositories
{
    public class SubjectEnrollmentDetailRepository:Repository<SubjectEnrollmentDetail>,ISubjectEnrollmentDetailRepository
    {
        public SubjectEnrollmentDetailRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            
        }
    }
}

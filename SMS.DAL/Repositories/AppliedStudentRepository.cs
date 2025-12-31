using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;

namespace SMS.DAL.Repositories
{
    public class AppliedStudentRepository : Repository<AppliedStudent>, IAppliedStudentRepository
    {
        public AppliedStudentRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}

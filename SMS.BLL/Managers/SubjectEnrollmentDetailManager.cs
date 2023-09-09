using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class SubjectEnrollmentDetailManager:Manager<SubjectEnrollmentDetail>, ISubjectEnrollmentDetailManager
    {
        public SubjectEnrollmentDetailManager(ISubjectEnrollmentDetailRepository subjectEnrollmentDetailRepository) :base(subjectEnrollmentDetailRepository)
        {
            
        }
    }
}

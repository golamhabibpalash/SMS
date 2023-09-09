using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class SubjectEnrollmentManager:Manager<SubjectEnrollment>, ISubjectEnrollmentManager
    {
        private readonly ISubjectEnrollmentRepository _subjectEnrollmentRepository;
        public SubjectEnrollmentManager(ISubjectEnrollmentRepository subjectEnrollmentRepository):base(subjectEnrollmentRepository)
        {
            _subjectEnrollmentRepository = subjectEnrollmentRepository;
        }

        public async Task<List<SubjectEnrollmentDetail>> GetAllDetailsByStudentIdAsync(int studentId)
        {
            List<SubjectEnrollmentDetail> result = new List<SubjectEnrollmentDetail>();
            try
            {
                result = await _subjectEnrollmentRepository.GetAllDetailsByStudentIdAsync(studentId);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<SubjectEnrollment> GetByStudentIdAsync(int studentId)
        {
            return await _subjectEnrollmentRepository.GetByStudentIdAsync(studentId);
        }
    }
}

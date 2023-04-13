using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class StudentPaymentDetailsManager : Manager<StudentPaymentDetails>, IStudentPaymentDetailsManager
    {
        private readonly IStudentPaymentDetailsRepository _studentPaymentDetailRepository;
        public StudentPaymentDetailsManager(IStudentPaymentDetailsRepository studentPaymentDetailsRepository): base(studentPaymentDetailsRepository)
        {
            _studentPaymentDetailRepository = studentPaymentDetailsRepository;
        }

        public async Task<List<StudentPaymentDetails>> GetAllByPaymentId(int studentPaymentId)
        {
            return await _studentPaymentDetailRepository.GetAllByPaymentId(studentPaymentId);
        }
    }
}

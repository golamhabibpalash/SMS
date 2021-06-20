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
        public StudentPaymentDetailsManager(IStudentPaymentDetailsRepository studentPaymentDetailsRepository): base(studentPaymentDetailsRepository)
        {

        }
    }
}

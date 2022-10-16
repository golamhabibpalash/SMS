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
    public class StudentPaymentManager : Manager<StudentPayment>, IStudentPaymentManager
    {
        private readonly IStudentPaymentRepository _studentPaymentRepository;
        private readonly IStudentRepository _studentRepository;
       
        public StudentPaymentManager(IStudentPaymentRepository studentPaymentRepository,IStudentRepository studentRepository) : base(studentPaymentRepository)
        {
            _studentPaymentRepository = studentPaymentRepository;
            _studentRepository = studentRepository;
        }

        public async Task<IReadOnlyCollection<StudentPayment>> GetAllByStudentIdAsync(int id)
        {
            return await _studentPaymentRepository.GetAllByStudentIdAsync(id);
        }

        //public async Task<IReadOnlyCollection<StudentPayment>> GetAllMonthPaymentsByThisDate(string date)
        //{
        //    var allPayments = await _studentPaymentRepository.GetAllAsync();
            
        //}

        public async Task<string> GetNewReceipt(int studentId, int feeHeadId)
        {
            string receiptsNo = string.Empty;
            Student student = await _studentRepository.GetByIdAsync(studentId);
            var allPayments = await _studentPaymentRepository.GetAllAsync();
            var sl = ((from p in allPayments
                      where p.PaidDate.ToString("yyMM") == DateTime.Today.ToString("yyMM")
                      select p).Count()+1).ToString().PadLeft(3,'0');
            receiptsNo = student.AcademicClassId.ToString()+feeHeadId.ToString()+DateTime.Now.ToString("yyMM")+sl;
            return receiptsNo;
        }
    }
}

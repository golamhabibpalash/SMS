using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
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
        public async Task<IReadOnlyCollection<StudentPaymentSummeryVM>> GetPaymentSummeryByDate(string date)
        {
            List<StudentPaymentSummeryVM> paymentSummery = new List<StudentPaymentSummeryVM>();
            try
            {
                paymentSummery = await _studentPaymentRepository.GetPaymentSummeryByDate(date);
            }
            catch (Exception)
            {

                throw;
            }
            return paymentSummery;
        }
        public async Task<IReadOnlyCollection<StudentPaymentSummeryVM>> GetPaymentSummeryByMonthYear(string monthYear)
        {
            List<StudentPaymentSummeryVM> paymentSummery = new List<StudentPaymentSummeryVM>();
            try
            {
                paymentSummery = await _studentPaymentRepository.GetPaymentSummeryByMonthYear(monthYear);
            }
            catch (Exception)
            {

                throw;
            }
            return paymentSummery;
        }
        public async Task<List<StudentPaymentScheduleVM>> GetStudentPaymentSchedule(int studId)
        {
            return await _studentPaymentRepository.GetStudentPaymentSchedule(studId);
        }
        public async Task<List<StudentPaymentSchedulePaidVM>> GetStudentPaymentSchedulePaid(int studId)
        {
            return await _studentPaymentRepository.GetStudentPaymentSchedulePaid(studId);
        }
    }
}

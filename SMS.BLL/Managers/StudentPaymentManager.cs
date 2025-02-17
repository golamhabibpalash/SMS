using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using SMS.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class StudentPaymentManager : Manager<StudentPayment>, IStudentPaymentManager
    {
        private readonly IStudentPaymentRepository _studentPaymentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassFeeListRepository _classFeeListRepository;
        private readonly IAcademicSessionRepository _academicSessionRepository;
        private readonly IStudentFeeHeadRepository _studentFeeHeadRepository;
        private readonly IStudentPaymentDetailsRepository _studentPaymentDetailsRepository;

        public StudentPaymentManager(IStudentPaymentRepository studentPaymentRepository, IStudentRepository studentRepository, IClassFeeListRepository classFeeListRepository, IAcademicSessionRepository academicSessionRepository, IStudentFeeHeadRepository studentFeeHeadRepository, IStudentPaymentDetailsRepository studentPaymentDetailsRepository) : base(studentPaymentRepository)
        {
            _studentPaymentRepository = studentPaymentRepository;
            _studentRepository = studentRepository;
            _classFeeListRepository = classFeeListRepository;
            _academicSessionRepository = academicSessionRepository;
            _studentFeeHeadRepository = studentFeeHeadRepository;
            _studentPaymentDetailsRepository = studentPaymentDetailsRepository;
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
                       select p).Count() + 1).ToString().PadLeft(3, '0');
            receiptsNo = student.AcademicClassId.ToString() + feeHeadId.ToString() + DateTime.Now.ToString("yyMM") + sl;

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

        public async Task<List<StudentPaymentSummerySMS_VM>> GetStudentPaymentSummerySMS_VMsAsync(DateTime date)
        {
            List<StudentPaymentSummerySMS_VM> paymentSummery = new List<StudentPaymentSummerySMS_VM>();
            try
            {
                paymentSummery = await _studentPaymentRepository.GetStudentPaymentSummerySMS_VMsAsync(date);
            }
            catch (Exception)
            {

                throw;
            }
            return paymentSummery;
        }

        public async Task<double> GetStudentCurrentDue(int stuId)
        {
            return await _studentPaymentRepository.GetStudentCurrentDue(stuId);
        }

        public async Task<List<StudentPayment>> GetPaymentByStudentUniqueId(string uniqueId)
        {
            List<StudentPayment> sps = new List<StudentPayment>();
            var paymens = await _studentPaymentRepository.GetAllAsync();
            sps = paymens.Where(p => p.UniqueId == uniqueId).ToList();
            return sps;
        }

        public async Task<List<PaidAmountResult>> GetPaidAmountByFeeHeadAsync(string uniqueId, int sessionId, int isResidential, int classId, int feeHeadId)
        {
            var result = await _studentPaymentRepository.GetPaidAmountByFeeHead(uniqueId, sessionId, isResidential, classId, feeHeadId);
            return result;
        }

        public async Task<StudentPaymentDetailVM> GetAllDetailPaymentByUniqueId(string studentUniqueId)
        {
            var student = await _studentRepository.GetStudentByUniqueIdAsync(studentUniqueId.Trim());
            var currentSession = await _academicSessionRepository.GetCurrentAcademicSession();
            if (student != null)
            {
                var studentPayments = await _studentPaymentRepository.GetAllByStudentUniqueIdAsync(studentUniqueId);
                StudentPaymentDetailVM studentPaymentDetailVM = new StudentPaymentDetailVM();
                var allSessions = await _academicSessionRepository.GetAllAsync();
                foreach (var session in allSessions.OrderByDescending(s => s.Name))
                {
                    var isPayment = studentPayments.Any(s => s.AcademicSessionId == session.Id);
                    var tAmount = await GetStudentTotalPaybleAmountBySessionAsync(session.Id, student.UniqueId);
                    var pAmount = await GetStudentTotalPaidAmountBySession(session.Id, student.UniqueId);
                    SinglePaymentVM singlePaymentVM = new SinglePaymentVM()
                    {
                        PaymentsTitle = "Detail Payments ",
                        AcademicSession = session.Name,
                        CurrentSession = currentSession.Name,
                        TotalAmount = tAmount,
                        TotalPaidAmount = pAmount,
                        TotalDueAmount = tAmount - pAmount,
                        SessionWisePaymentVMs = await GetSessionWisePaymentVMs(session.Id, studentUniqueId)
                    };
                    studentPaymentDetailVM.Payments.Add(singlePaymentVM);
                }
                return studentPaymentDetailVM;
            }
            return null;
        }
        private async Task<List<SessionWisePaymentVM>> GetSessionWisePaymentVMs(int sessionId, string uniqueId)
        {
            var student = await _studentRepository.GetStudentByUniqueIdAsync(uniqueId);
            List<SessionWisePaymentVM> sessionWisePaymentVMs = new List<SessionWisePaymentVM>();
            var allClassFees = await _classFeeListRepository.GetAllBySessionIdClassIdAsync(sessionId, student.AcademicClassId);
            allClassFees = allClassFees.Where(s => s.StudentFeeHead.IsResidential == student.IsResidential).ToList();
            var allPaymentDetailsByStudent = await _studentPaymentDetailsRepository.GetAllByStudentAsync(uniqueId);

            if (allClassFees != null)
            {
                foreach (var classFee in allClassFees.OrderBy(s => s.SL))
                {
                    var classFeeList = await _classFeeListRepository.GetClassFeeListByClassIdFeeHeadIdSessionIdAsync(student.AcademicClassId, classFee.StudentFeeHeadId, sessionId);
                    var totalAmount = classFeeList.Select(s => s.Amount).FirstOrDefault();
                    var paidAmount = allPaymentDetailsByStudent.Where(d => d.ClassFeeId == classFeeList.Select(c => c.Id).FirstOrDefault() && d.StudentFeeHeadId == classFee.StudentFeeHeadId).Select(s => s.PaidAmount).Sum();

                    SessionWisePaymentVM sessionWisePaymentVM = new SessionWisePaymentVM()
                    {
                        FeeHeadName = classFee.StudentFeeHead.Name,
                        Amount = totalAmount,
                        PaidAmount = paidAmount,
                        Balance = totalAmount - paidAmount,
                        Status = GetPaymentStatus(totalAmount, paidAmount),
                        SessionWisePaymentDetails = await GetSessionWisePaymentDetails(classFee.StudentFeeHeadId, uniqueId, sessionId, totalAmount)
                    };
                    sessionWisePaymentVMs.Add(sessionWisePaymentVM);
                }
                return sessionWisePaymentVMs;
            }
            return null;
        }

        private async Task<List<SessionWisePaymentDetails>> GetSessionWisePaymentDetails(int feeHeadId, string uniqueId, int sessionId, double totalAmount)
        {
            var sPaymentDetails = await _studentPaymentDetailsRepository.GetAllByStudentAsync(uniqueId);
            sPaymentDetails = sPaymentDetails.Where(p => p.StudentPayment.AcademicSessionId == sessionId && p.StudentFeeHeadId == feeHeadId).ToList();
            var paymentDetails = new List<SessionWisePaymentDetails>();
            if (sPaymentDetails != null)
            {
                var totalPaid = 0.0;
                var paybleAmont = 0.0;
                foreach (var item in sPaymentDetails)
                {
                    paybleAmont = totalAmount - totalPaid;
                    totalPaid += item.PaidAmount;
                    var pAmount = item.PaidAmount;
                    var restAmount = totalAmount - totalPaid;
                    SessionWisePaymentDetails sessionWisePaymentDetails = new SessionWisePaymentDetails()
                    {
                        PaymentDetailId = item.Id,
                        PaidDate = item.CreatedAt.ToString("dd MMM yyyy"),
                        ReceiptNo = item.StudentPayment.ReceiptNo,
                        PayableAmount = paybleAmont,
                        PaidAmount = item.PaidAmount,
                        DueAmount = restAmount,
                        Remarks = item.StudentPayment.Remarks,
                        Status = GetPaymentStatus(paybleAmont, pAmount),
                        PaymentId = item.StudentPayment.Id
                    };
                    paymentDetails.Add(sessionWisePaymentDetails);
                };
            }
            return paymentDetails;
        }

        private string GetPaymentStatus(double amount, double paidAmount)
        {
            var status = string.Empty;
            if (paidAmount == 0 && (amount > paidAmount))
            {
                status = StudentPaymentStatus.Unpaid.ToString();
            }
            else if (amount > paidAmount)
            {
                status = StudentPaymentStatus.Partial.ToString();
            }
            else if (amount == paidAmount || amount < paidAmount)
            {
                status = StudentPaymentStatus.Paid.ToString();
            }
            return status;
        }
        public async Task<double> GetStudentTotalPaybleAmountBySessionAsync(int sessionId, string studentUniqueId)
        {
            var totalFees = 0.00;
            var student = await _studentRepository.GetStudentByUniqueIdAsync(studentUniqueId);
            var allClassFeeBySessionId = await _classFeeListRepository.GetAllBySessionIdClassIdAsync(sessionId, student.AcademicClassId);
            if (allClassFeeBySessionId != null)
            {
                var allFeeHead = await _studentFeeHeadRepository.GetAllAsync();
                totalFees = allClassFeeBySessionId.Where(c => c.StudentFeeHead.IsResidential == student.IsResidential).Select(s => s.Amount).Sum();
            }
            return totalFees;
        }
        public async Task<double> GetStudentTotalPaidAmountBySession(int sessionId, string studentUniqueId)
        {
            var student = await _studentRepository.GetStudentByUniqueIdAsync(studentUniqueId);
            var allPayments = await GetPaymentByStudentUniqueId(studentUniqueId);
            var amount = allPayments.Where(s => s.AcademicSessionId == sessionId).Select(s => s.TotalPayment).Sum();
            return amount;
        }
        public async Task<double> GetStudentTotalDueAmountBySession(int sessionId, string studentUniqueId)
        {
            var student = await _studentRepository.GetStudentByUniqueIdAsync(studentUniqueId);
            var currentDues = await GetStudentCurrentDue(student.Id);
            return currentDues;
        }
    }
}

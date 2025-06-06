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
        private readonly IStudentFeeAllocationRepository _studentFeeAllocationRepository;

        public StudentPaymentManager(IStudentPaymentRepository studentPaymentRepository, IStudentRepository studentRepository, IClassFeeListRepository classFeeListRepository, IAcademicSessionRepository academicSessionRepository, IStudentFeeHeadRepository studentFeeHeadRepository, IStudentPaymentDetailsRepository studentPaymentDetailsRepository, IStudentFeeAllocationRepository studentFeeAllocationRepository) : base(studentPaymentRepository)
        {
            _studentPaymentRepository = studentPaymentRepository;
            _studentRepository = studentRepository;
            _classFeeListRepository = classFeeListRepository;
            _academicSessionRepository = academicSessionRepository;
            _studentFeeHeadRepository = studentFeeHeadRepository;
            _studentPaymentDetailsRepository = studentPaymentDetailsRepository;
            _studentFeeAllocationRepository = studentFeeAllocationRepository;
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
            var student = await _studentRepository.GetByIdAsync(stuId);
            var currentMonth = DateTime.Now.Month;
            double currentDue = 0.00;

            var currentSession = await _academicSessionRepository.GetCurrentAcademicSession();
            var classFees = await _classFeeListRepository.GetAllBySessionIdClassIdAsync(student.AcademicSessionId, student.AcademicClassId);
            classFees = classFees.Where(s => s.StudentFeeHead.IsResidential == student.IsResidential).ToList();
            var allAllocations = await _studentFeeAllocationRepository.GetStudentFeeAllocationByUniqueIdSessionId(student.UniqueId, student.AcademicSessionId);
            allAllocations = allAllocations.Where(s => s.IsActive == true).ToList();
            //Admission or Session Fee
            bool isAdmissionFee = student.AdmissionDate.Year.ToString() == currentSession.Name[^4..];
            int feeHeadSl = isAdmissionFee ? 0 : 13;
            var AdmissionOrSessionClassFee = classFees.FirstOrDefault(s => s.StudentFeeHead.SL == feeHeadSl);
            var admissionOrSessionAllocation = allAllocations.FirstOrDefault(s => s.UniqueId == student.UniqueId && s.ClassFeeListId == AdmissionOrSessionClassFee.Id);             
            var admissionOrSessionSheduledFee = GetAdmissionOrSessionScheduleFee(student.AcademicSessionId, student.AcademicClassId, student.UniqueId, AdmissionOrSessionClassFee ,admissionOrSessionAllocation);

            //monthly fee
            var monthlyAllocations = allAllocations.Where(s => s.StudentFeeHead.SL>=1 && s.StudentFeeHead.SL<=12).ToList();
            var monthlyClassFees = classFees.Where(s => s.StudentFeeHead.SL >= 1 && s.StudentFeeHead.SL <= 12).ToList();
            var monthlyScheduledFee = GetMonthlyScheduledFee(student.AcademicClassId, student.AcademicSessionId, student.UniqueId, monthlyClassFees, monthlyAllocations);

            //others fee
            var othersAllocation = allAllocations.Where(s => s.StudentFeeHead.SL >= 13).ToList();
            var otherClassFees = classFees.Where(s => s.StudentFeeHead.SL >= 13).ToList();
            var othersScheduleFee = GetOtherScheduledFee(student.AcademicClassId, student.AcademicSessionId, student.UniqueId, otherClassFees, othersAllocation);

            var totalScheduledFees = admissionOrSessionSheduledFee + monthlyScheduledFee + othersScheduleFee;
            var totalPaidFees =await GetStudentTotalPaidAmountBySession(student.AcademicSessionId, student.UniqueId);
            currentDue = totalScheduledFees - totalPaidFees;

            return currentDue;
        }
        private double GetAdmissionOrSessionScheduleFee(int sessionId, int classId, string uniqueId, ClassFeeList AdmissionOrSessionClassFee, StudentFeeAllocation allocation)
        {
            //admission/session fee
            // SL 0 = admission fee & 13 = session fee
            double admissionOrSessionFee = 0.0;
            
             //0 is for admission and 13 is for session

            if (AdmissionOrSessionClassFee != null)
            {
                
                admissionOrSessionFee = allocation?.AllocatedAmount ?? AdmissionOrSessionClassFee.Amount;
            }
            return admissionOrSessionFee;
        }
        private double GetMonthlyScheduledFee(int classId, int sessionId, string uniqueId, List<ClassFeeList> monthlyClassFees, List<StudentFeeAllocation> allAllocations)
        {
            var monthlyPaymentFee = 0.0;
            var currentMonth = DateTime.Now.Month;
            foreach (var classFee in monthlyClassFees.OrderBy(s => s.StudentFeeHead.SL))
            {
                var expectedAllocation  = allAllocations.FirstOrDefault(s => s.ClassFeeListId == classFee.Id);
                monthlyPaymentFee += expectedAllocation != null ? expectedAllocation.AllocatedAmount : classFee.Amount;
                if (classFee.StudentFeeHead.SL > currentMonth)
                {
                    break;
                }
            }

            return monthlyPaymentFee;
        }
        private double GetOtherScheduledFee(int classId, int sessionId, string uniqueId, List<ClassFeeList> othersClassFees, List<StudentFeeAllocation> allAllocations)
        {
            var otherScheduledFee = 0.0;
            var currentMonth = DateTime.Now.Month;
            foreach (var classFee in othersClassFees)
            {
                var expectedAllocation = allAllocations.FirstOrDefault(s => s.ClassFeeListId == classFee.Id);
                otherScheduledFee += expectedAllocation != null ? expectedAllocation.AllocatedAmount : classFee.Amount;
                if (classFee.StudentFeeHead.SL > currentMonth)
                {
                    break;
                }
            }

            return otherScheduledFee;
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
                    if (isPayment)
                    {
                        var sessionWisePaymentDetails = studentPayments.Where(s => s.AcademicSessionId == session.Id).Select(s => s.StudentPaymentDetails).ToList();
                        var classFeeId = sessionWisePaymentDetails.FirstOrDefault().Select(s => s.ClassFeeId).FirstOrDefault();
                        var classFee = await _classFeeListRepository.GetByIdAsync(classFeeId);

                        var tAmount = await GetStudentTotalPaybleAmountBySessionAsync(session.Id, student.UniqueId, classFee.AcademicClassId);
                        var pAmount = await GetStudentTotalPaidAmountBySession(session.Id, student.UniqueId);
                        SinglePaymentVM singlePaymentVM = new SinglePaymentVM()
                        {
                            PaymentsTitle = "Detail Payments ",
                            AcademicSession = session.Name,
                            CurrentSession = currentSession.Name,
                            TotalAmount = tAmount,
                            TotalPaidAmount = pAmount,
                            TotalDueAmount = tAmount - pAmount,
                            SessionWisePaymentVMs = await GetSessionWisePaymentVMs(session.Id, studentUniqueId, classFee.AcademicClassId)
                        };
                        studentPaymentDetailVM.Payments.Add(singlePaymentVM);

                    }
                }
                return studentPaymentDetailVM;
            }
            return null;
        }
        private async Task<List<SessionWisePaymentVM>> GetSessionWisePaymentVMs(int sessionId, string uniqueId, int classId)
        {
            var student = await _studentRepository.GetStudentByUniqueIdAsync(uniqueId);
            List<SessionWisePaymentVM> sessionWisePaymentVMs = new List<SessionWisePaymentVM>();
            var allClassFees = await _classFeeListRepository.GetAllBySessionIdClassIdAsync(sessionId, classId);
            allClassFees = allClassFees.Where(s => s.StudentFeeHead.IsResidential == student.IsResidential).ToList();
            var allPaymentDetailsByStudent = await _studentPaymentDetailsRepository.GetAllByStudentAsync(uniqueId);
            var admissionYear = student.AdmissionDate.Year.ToString();
            var nowSession = await _academicSessionRepository.GetByIdAsync(sessionId);
            var sessionYear = nowSession.Name.Substring(nowSession.Name.Length - 4, 4);
            if (admissionYear == sessionYear)
            {

            }
            else
            {

            }
            if (allClassFees != null)
            {
                foreach (var classFee in allClassFees.OrderBy(s => s.SL))
                {
                    var classFeeList = await _classFeeListRepository.GetClassFeeListByClassIdFeeHeadIdSessionIdAsync(classId, classFee.StudentFeeHeadId, sessionId);
                    //Admin or Session
                    var isAdmittedByThisSession = allPaymentDetailsByStudent.Any(s => s.StudentFeeHeadId == 2 || s.StudentFeeHeadId == 6);

                    if (isAdmittedByThisSession)
                    {
                        if (classFee.StudentFeeHead.Name == "Session Fee")
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (classFee.StudentFeeHead.Name == "Admission Fee")
                        {
                            continue;
                        }
                    }

                    var totalAmount = classFeeList.Select(s => s.Amount).FirstOrDefault();
                    var allocations = await _studentFeeAllocationRepository.GetStudentFeeAllocationByUniqueIdFeeHeadId(uniqueId,classFeeList.Select(s => s.StudentFeeHeadId).FirstOrDefault());
                    if (allocations != null)
                    {
                        totalAmount = allocations.AllocatedAmount;
                    }
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
                }
                ;
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

        public async Task<double> GetStudentTotalPaybleAmountBySessionAsync(int sessionId, string studentUniqueId, int classId)
        {
            var totalFees = 0.00;
            var student = await _studentRepository.GetStudentByUniqueIdAsync(studentUniqueId);
            var allClassFeeBySessionId = await _classFeeListRepository
                .GetAllBySessionIdClassIdAsync(sessionId, classId, student.IsResidential);

            List<ClassFeeList> classFeeLists = new List<ClassFeeList>();


            //Admin or Session
            DateTime addmissionYear = student.AdmissionDate;
            AcademicSession academicSession = await _academicSessionRepository.GetByIdAsync(sessionId);

            var studentAllAllocations = await _studentFeeAllocationRepository.GetStudentFeeAllocationByUniqueIdSessionId(studentUniqueId, sessionId);
            if (allClassFeeBySessionId != null)
            {
                foreach (var cFee in allClassFeeBySessionId)
                {

                    if (addmissionYear.Year == Convert.ToInt32(academicSession.Name.Substring((academicSession.Name.Length - 4), 4)))
                    {
                        if (cFee.StudentFeeHead.Name == "Session Fee")
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (cFee.StudentFeeHead.Name == "Admission Fee")
                        {
                            continue;
                        }
                    }
                    var allocation =studentAllAllocations.FirstOrDefault(s => s.StudentFeeHeadId == cFee.StudentFeeHeadId);
                    if (allocation!=null)
                    {
                        cFee.Amount = allocation.AllocatedAmount;
                    }
                    classFeeLists.Add(cFee);
                }

                if (classFeeLists.Count > 0)
                {
                    totalFees = classFeeLists
                        .Where(c => c.StudentFeeHead.IsResidential == student.IsResidential)
                        .Select(s => s.Amount)
                        .Sum();
                }
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
        private List<ClassFeeList> FilterWithAdmissionSessionFee(string admissionYear, string sessionYear, List<ClassFeeList> classFeeLists)
        {
            if (admissionYear == sessionYear)
            {

                //Remove Session Fee
                classFeeLists = classFeeLists.Where(c => !c.StudentFeeHead.Name.Contains("Session Fee")).ToList();
            }
            else
            {
                //Remove Admission Fee

            }
            return classFeeLists;
        }
    }
}

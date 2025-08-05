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

namespace SMS.BLL.Managers;

public class StudentPaymentManager : Manager<StudentPayment>, IStudentPaymentManager
{
    private readonly IStudentPaymentRepository _studentPaymentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IClassFeeListRepository _classFeeListRepository;
    private readonly IAcademicSessionRepository _academicSessionRepository;
    private readonly IStudentFeeHeadRepository _studentFeeHeadRepository;
    private readonly IStudentPaymentDetailsRepository _studentPaymentDetailsRepository;
    private readonly IStudentFeeAllocationRepository _studentFeeAllocationRepository;
    private readonly IParamBusConfigManager _paramBusConfigManager;

    public StudentPaymentManager(
        IStudentPaymentRepository studentPaymentRepository,
        IStudentRepository studentRepository,
        IClassFeeListRepository classFeeListRepository,
        IAcademicSessionRepository academicSessionRepository,
        IStudentFeeHeadRepository studentFeeHeadRepository,
        IStudentPaymentDetailsRepository studentPaymentDetailsRepository,
        IStudentFeeAllocationRepository studentFeeAllocationRepository,
        IParamBusConfigManager paramBusConfigManager)
        : base(studentPaymentRepository)
    {
        _studentPaymentRepository = studentPaymentRepository;
        _studentRepository = studentRepository;
        _classFeeListRepository = classFeeListRepository;
        _academicSessionRepository = academicSessionRepository;
        _studentFeeHeadRepository = studentFeeHeadRepository;
        _studentPaymentDetailsRepository = studentPaymentDetailsRepository;
        _studentFeeAllocationRepository = studentFeeAllocationRepository;
        _paramBusConfigManager = paramBusConfigManager;
    }

    public async Task<IReadOnlyCollection<StudentPayment>> GetAllByStudentIdAsync(int id)
    {
        return await _studentPaymentRepository.GetAllByStudentIdAsync(id);
    }

    public async Task<string> GetNewReceipt(int studentId, int feeHeadId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null) return string.Empty;

        var allPayments = await _studentPaymentRepository.GetAllAsync();
        var paymentCount = allPayments
            .Count(p => p.PaidDate.ToString("yyMM") == DateTime.Today.ToString("yyMM")) + 1;
        var serial = paymentCount.ToString().PadLeft(3, '0');

        return $"{student.AcademicClassId}{feeHeadId}{DateTime.Now:yyMM}{serial}";
    }

    public async Task<IReadOnlyCollection<StudentPaymentSummeryVM>> GetPaymentSummeryByDate(string date)
    {
        try
        {
            return await _studentPaymentRepository.GetPaymentSummeryByDate(date);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IReadOnlyCollection<StudentPaymentSummeryVM>> GetPaymentSummeryByMonthYear(string monthYear)
    {
        try
        {
            return await _studentPaymentRepository.GetPaymentSummeryByMonthYear(monthYear);
        }
        catch (Exception)
        {
            throw;
        }
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
        try
        {
            return await _studentPaymentRepository.GetStudentPaymentSummerySMS_VMsAsync(date);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<double> GetStudentCurrentDue(int stuId)
    {
        var student = await _studentRepository.GetByIdAsync(stuId);
        if (student == null) return 0;

        var session = await _academicSessionRepository.GetCurrentAcademicSession();
        if (session == null) return 0;

        var classFees = await GetFilteredClassFeesAsync(student);
        var allocations = await GetFilteredAllocationsAsync(student);

        double totalScheduledFee = CalculateTotalScheduledFee(student, session, classFees, allocations);
        double totalPaid = await GetStudentTotalPaidAmountBySession(session.Id, student.UniqueId);

        return totalScheduledFee - totalPaid;
    }

    public async Task<List<StudentPayment>> GetPaymentByStudentUniqueId(string uniqueId)
    {
        var payments = await _studentPaymentRepository.GetAllAsync();
        return payments.Where(p => p.UniqueId == uniqueId).ToList();
    }

    public async Task<List<PaidAmountResult>> GetPaidAmountByFeeHeadAsync(string uniqueId, int sessionId, int isResidential, int classId, int feeHeadId)
    {
        return await _studentPaymentRepository.GetPaidAmountByFeeHead(uniqueId, sessionId, isResidential, classId, feeHeadId);
    }

    public async Task<StudentPaymentDetailVM> GetAllDetailPaymentByUniqueId(string studentUniqueId)
    {
        var student = await _studentRepository.GetStudentByUniqueIdAsync(studentUniqueId.Trim());
        if (student == null) return null;

        var currentSession = await _academicSessionRepository.GetCurrentAcademicSession();
        var studentPayments = await _studentPaymentRepository.GetAllByStudentUniqueIdAsync(studentUniqueId);
        var allSessions = await _academicSessionRepository.GetAllAsync();

        var studentPaymentDetailVM = new StudentPaymentDetailVM();
        foreach (var session in allSessions.OrderByDescending(s => s.Name))
        {
            if (!studentPayments.Any(s => s.AcademicSessionId == session.Id)) continue;

            var sessionWisePaymentDetails = studentPayments
                .Where(s => s.AcademicSessionId == session.Id)
                .SelectMany(s => s.StudentPaymentDetails)
                .ToList();
            var classFeeId = sessionWisePaymentDetails.FirstOrDefault()?.ClassFeeId ?? 0;
            var classFee = await _classFeeListRepository.GetByIdAsync(classFeeId);

            var singlePaymentVM = new SinglePaymentVM
            {
                PaymentsTitle = "Detail Payments",
                AcademicSession = session.Name,
                CurrentSession = currentSession.Name,
                TotalAmount = await GetStudentTotalPaybleAmountBySessionAsync(session.Id, student.UniqueId, classFee?.AcademicClassId ?? 0),
                TotalPaidAmount = await GetStudentTotalPaidAmountBySession(session.Id, student.UniqueId),
                TotalDueAmount = await GetStudentCurrentDue(student.Id),
                SessionWisePaymentVMs = await GetSessionWisePaymentVMs(session.Id, studentUniqueId, classFee?.AcademicClassId ?? 0)
            };
            studentPaymentDetailVM.Payments.Add(singlePaymentVM);
        }

        return studentPaymentDetailVM;
    }

    public async Task<double> GetStudentTotalPaybleAmountBySessionAsync(int sessionId, string studentUniqueId, int classId)
    {
        var student = await _studentRepository.GetStudentByUniqueIdAsync(studentUniqueId);
        if (student == null) return 0;

        var classFees = await _classFeeListRepository.GetAllBySessionIdClassIdAsync(sessionId, classId, student.IsResidential);
        if (classFees == null || !classFees.Any()) return 0;

        var allocations = await _studentFeeAllocationRepository.GetStudentFeeAllocationByUniqueIdSessionId(studentUniqueId, sessionId);
        var admissionYear = student.AdmissionDate.Year.ToString();
        var sessionYear = (await _academicSessionRepository.GetByIdAsync(sessionId))?.Name[^4..];

        var filteredFees = FilterFeesByAdmissionAndSession(classFees, admissionYear, sessionYear, allocations, student.IsResidential, student.AdmissionDate.Month);
        return filteredFees.Sum(c => c.Amount);
    }

    public async Task<double> GetStudentTotalPaidAmountBySession(int sessionId, string studentUniqueId)
    {
        var payments = await GetPaymentByStudentUniqueId(studentUniqueId);
        return payments.Where(s => s.AcademicSessionId == sessionId).Sum(s => s.TotalPayment);
    }

    public async Task<double> GetStudentTotalDueAmountBySession(int sessionId, string studentUniqueId)
    {
        var student = await _studentRepository.GetStudentByUniqueIdAsync(studentUniqueId);
        return await GetStudentCurrentDue(student?.Id ?? 0);
    }

    private async Task<List<ClassFeeList>> GetFilteredClassFeesAsync(Student student)
    {
        return await _classFeeListRepository.GetByClassIdSessionIdStudentIdAsync(
            student.AcademicClassId, student.AcademicSessionId, student.Id);
    }

    private async Task<List<StudentFeeAllocation>> GetFilteredAllocationsAsync(Student student)
    {
        return (await _studentFeeAllocationRepository
            .GetStudentFeeAllocationByUniqueIdSessionId(student.UniqueId, student.AcademicSessionId))
            .Where(a => a.IsActive)
            .ToList();
    }

    private double CalculateTotalScheduledFee(Student student, AcademicSession session, List<ClassFeeList> classFees, List<StudentFeeAllocation> allocations)
    {
        double admissionOrSessionFee = CalculateAdmissionOrSessionFee(student, session, classFees, allocations);
        double monthlyFee = CalculateMonthlyFee(student, classFees, allocations);
        double otherFee = CalculateOtherFee(student, classFees, allocations);

        return admissionOrSessionFee + monthlyFee + otherFee;
    }

    private double CalculateAdmissionOrSessionFee(Student student, AcademicSession session, List<ClassFeeList> classFees, List<StudentFeeAllocation> allocations)
    {
        bool isAdmission = student.AdmissionDate.Year.ToString() == session.Name[^4..];
        int feeSl = isAdmission ? 0 : 13;
        var classFee = classFees.FirstOrDefault(f => f.StudentFeeHead.SL == feeSl);
        var allocation = allocations.FirstOrDefault(a => a.ClassFeeListId == classFee?.Id);

        return classFee != null ? (allocation?.AllocatedAmount ?? classFee.Amount) : 0;
    }

    private double CalculateMonthlyFee(Student student, List<ClassFeeList> classFees, List<StudentFeeAllocation> allocations)
    {
        var monthlyFees = classFees
            .Where(c => c.StudentFeeHead.SL is >= 1 and <= 12 && ShouldIncludeFeeForMonth(c, student.AdmissionDate))
            .ToList();
        var monthlyAllocations = allocations.Where(a => a.StudentFeeHead.SL is >= 1 and <= 12).ToList();

        return monthlyFees.Sum(fee => monthlyAllocations.FirstOrDefault(a => a.ClassFeeListId == fee.Id)?.AllocatedAmount ?? fee.Amount);
    }

    private double CalculateOtherFee(Student student, List<ClassFeeList> classFees, List<StudentFeeAllocation> allocations)
    {
        var otherFees = classFees.Where(f => f.StudentFeeHead.SL > 13).ToList();
        var otherAllocations = allocations.Where(a => a.StudentFeeHead.SL > 13).ToList();

        return otherFees.Sum(fee => otherAllocations.FirstOrDefault(a => a.ClassFeeListId == fee.Id)?.AllocatedAmount ?? fee.Amount);
    }

    private bool ShouldIncludeFeeForMonth(ClassFeeList fee, DateTime admissionDate)
    {
        if (int.TryParse(fee.AcademicSession.Name[^4..], out int sessionYear) && sessionYear == admissionDate.Year)
        {
            return fee.SL >= admissionDate.Month;
        }
        return true;
    }

    private async Task<List<SessionWisePaymentVM>> GetSessionWisePaymentVMs(int sessionId, string uniqueId, int classId)
    {
        var student = await _studentRepository.GetStudentByUniqueIdAsync(uniqueId);
        var allParamConfig = await _paramBusConfigManager.GetAllAsync();
        var sessionFeeName = student.IsResidential ? allParamConfig.FirstOrDefault(s => s.ParamSL == 17)?.ParamValue : allParamConfig.FirstOrDefault(s => s.ParamSL == 16)?.ParamValue;
        var admissionFeeName = student.IsResidential ? allParamConfig.FirstOrDefault(s => s.ParamSL == 15)?.ParamValue : allParamConfig.FirstOrDefault(s => s.ParamSL == 14)?.ParamValue;

        var classFees = await _classFeeListRepository.GetAllBySessionIdClassIdAsync(sessionId, classId);
        classFees = classFees.Where(s => s.StudentFeeHead.IsResidential == student.IsResidential).ToList();
        var allAllocations = await _studentFeeAllocationRepository.GetStudentFeeAllocationByUniqueIdSessionId(uniqueId, sessionId);
        var paymentDetails = await _studentPaymentDetailsRepository.GetAllByStudentAsync(uniqueId);

        var sessionWisePaymentVMs = new List<SessionWisePaymentVM>();
        var admissionYear = student.AdmissionDate.Year.ToString();
        var sessionYear = (await _academicSessionRepository.GetByIdAsync(sessionId))?.Name[^4..];

        foreach (var classFee in classFees.OrderBy(s => s.SL))
        {
            if (ShouldSkipFee(classFee, student, admissionYear, sessionYear, paymentDetails, sessionFeeName, admissionFeeName)) continue;

            var classFeeList = await _classFeeListRepository.GetClassFeeListByClassIdFeeHeadIdSessionIdAsync(classId, classFee.StudentFeeHeadId, sessionId);
            var totalAmount = classFeeList.FirstOrDefault()?.Amount ?? 0;
            var allocation = allAllocations.FirstOrDefault(s => s.ClassFeeListId == classFee.Id);
            if (allocation != null) totalAmount = allocation.AllocatedAmount;

            var paidAmount = paymentDetails
                .Where(d => d.ClassFeeId == classFeeList.FirstOrDefault()?.Id && d.StudentFeeHeadId == classFee.StudentFeeHeadId)
                .Sum(s => s.PaidAmount);

            sessionWisePaymentVMs.Add(new SessionWisePaymentVM
            {
                FeeHeadName = classFee.StudentFeeHead.Name,
                Amount = totalAmount,
                PaidAmount = paidAmount,
                Balance = totalAmount - paidAmount,
                Status = GetPaymentStatus(totalAmount, paidAmount),
                SessionWisePaymentDetails = await GetSessionWisePaymentDetails(classFee.StudentFeeHeadId, uniqueId, sessionId, totalAmount)
            });
        }

        return sessionWisePaymentVMs.Any() ? sessionWisePaymentVMs : null;
    }

    private bool ShouldSkipFee(ClassFeeList classFee, Student student, string admissionYear, string sessionYear, List<StudentPaymentDetails> paymentDetails, string sessionFeeName, string admissionFeeName)
    {
        bool isAdmittedByThisSession = admissionYear == sessionYear;
        if (isAdmittedByThisSession && classFee.StudentFeeHead.Name == sessionFeeName) return true;
        if (!isAdmittedByThisSession && classFee.StudentFeeHead.Name == admissionFeeName) return true;
        if (admissionYear == sessionYear && classFee.SL is >= 1 and <= 12 && classFee.SL < student.AdmissionDate.Month) return true;
        return false;
    }

    private async Task<List<SessionWisePaymentDetails>> GetSessionWisePaymentDetails(int feeHeadId, string uniqueId, int sessionId, double totalAmount)
    {
        var paymentDetails = await _studentPaymentDetailsRepository.GetAllByStudentAsync(uniqueId);
        paymentDetails = paymentDetails.Where(p => p.StudentPayment.AcademicSessionId == sessionId && p.StudentFeeHeadId == feeHeadId).ToList();

        var result = new List<SessionWisePaymentDetails>();
        double totalPaid = 0;

        foreach (var item in paymentDetails)
        {
            var payableAmount = totalAmount - totalPaid;
            totalPaid += item.PaidAmount;

            result.Add(new SessionWisePaymentDetails
            {
                PaymentDetailId = item.Id,
                PaidDate = item.CreatedAt.ToString("dd MMM yyyy"),
                ReceiptNo = item.StudentPayment.ReceiptNo,
                PayableAmount = payableAmount,
                PaidAmount = item.PaidAmount,
                DueAmount = totalAmount - totalPaid,
                Remarks = item.StudentPayment.Remarks,
                Status = GetPaymentStatus(payableAmount, item.PaidAmount),
                PaymentId = item.StudentPayment.Id
            });
        }

        return result;
    }

    private string GetPaymentStatus(double amount, double paidAmount)
    {
        if (paidAmount == 0 && amount > paidAmount) return StudentPaymentStatus.Unpaid.ToString();
        if (amount > paidAmount) return StudentPaymentStatus.Partial.ToString();
        return StudentPaymentStatus.Paid.ToString();
    }

    private List<ClassFeeList> FilterFeesByAdmissionAndSession(List<ClassFeeList> classFees, string admissionYear, string sessionYear, List<StudentFeeAllocation> allocations, bool isResidential, int admissionMonth)
    {
        var filteredFees = new List<ClassFeeList>();
        foreach (var fee in classFees.Where(f => f.StudentFeeHead.IsResidential == isResidential))
        {
            bool isAdmissionYear = admissionYear == sessionYear;
            if (isAdmissionYear && fee.SL is >= 1 and <= 12 && fee.SL < admissionMonth) continue;
            if (!isAdmissionYear && fee.StudentFeeHead.Name == "Admission Fee") continue;
            if (isAdmissionYear && fee.StudentFeeHead.Name == "Session Fee") continue;

            var allocation = allocations.FirstOrDefault(s => s.StudentFeeHeadId == fee.StudentFeeHeadId);
            if (allocation != null) fee.Amount = allocation.AllocatedAmount;
            filteredFees.Add(fee);
        }
        return filteredFees;
    }
}
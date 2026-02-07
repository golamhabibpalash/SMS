using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using SMS.Entities.AdditionalModels.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories;

public class StudentPaymentRepository : Repository<StudentPayment>, IStudentPaymentRepository
{
    private readonly new ApplicationDbContext _context;

    public StudentPaymentRepository(ApplicationDbContext db) : base(db)
    {
        _context = db;
    }
    public override async Task<IReadOnlyCollection<StudentPayment>> GetAllAsync()
    {
        List<StudentPayment> payments = new List<StudentPayment>();
        try
        {
            payments = await _context
            .StudentPayment
            .Include(sp => sp.Student)
            .Include(sp => sp.StudentPaymentDetails)
            .ToListAsync();
        }
        catch (Exception)
        {

            throw;
        }

        return payments;
    }
    public async Task<IReadOnlyCollection<StudentPayment>> GetAllByStudentIdAsync(int id)
    {
        List<StudentPayment> payments = new List<StudentPayment>();
        try
        {
            payments = await _context.StudentPayment
            .Include(sp => sp.StudentPaymentDetails)
                .ThenInclude(sp => sp.StudentFeeHead)
            .Include(s => s.Student)
                .ThenInclude(ss => ss.AcademicClass)
            .Include(s => s.Student.AcademicSession)
            .Where(sp => sp.StudentId == id).ToListAsync();
        }
        catch (Exception)
        {

            throw;
        }
        return payments;
    }
    public async Task<List<StudentPaymentSummeryVM>> GetPaymentSummeryByDate(string date)
    {
        List<StudentPaymentSummeryVM> payments = new List<StudentPaymentSummeryVM>();
        try
        {
            payments = await _context.StudentPaymentSummeryVMs.FromSqlInterpolated($"sp_get_payWithClass_by_date {date}").ToListAsync();
        }
        catch (Exception)
        {

            throw;
        }


        return payments;
    }
    public async Task<List<StudentPaymentSummerySMS_VM>> GetStudentPaymentSummerySMS_VMsAsync(DateTime date)
    {
        List<StudentPaymentSummerySMS_VM> payments = new List<StudentPaymentSummerySMS_VM>();
        string pDate = date.ToString("yyyyMMdd");
        try
        {
            payments = await _context.studentPaymentSummerySMS_VMs.FromSqlInterpolated($"sp_Get_PaymentSummery_Daily_SMS {pDate}").ToListAsync();
        }
        catch (Exception)
        {

            throw;
        }


        return payments;
    }
    public async Task<List<StudentPaymentSummeryVM>> GetPaymentSummeryByMonthYear(string monthYear)
    {
        List<StudentPaymentSummeryVM> payments = new List<StudentPaymentSummeryVM>();
        try
        {
            payments = await _context.StudentPaymentSummeryVMs.FromSqlInterpolated($"sp_get_payWithClass_by_monthyear {monthYear}").ToListAsync();
        }
        catch (Exception)
        {

            throw;
        }

        return payments;
    }
    public override async Task<StudentPayment> GetByIdAsync(int id)
    {
        StudentPayment existingStudentPayment = await _context.StudentPayment
            .Include(s => s.Student)
                .ThenInclude(s => s.AcademicClass)
            .Include(s => s.Student.AcademicSession)
            .Include(s => s.StudentPaymentDetails)
                .ThenInclude(d => d.StudentFeeHead)
            .Where(s => s.Id == id).FirstOrDefaultAsync();

        return existingStudentPayment;
    }
    public async Task<List<StudentPaymentScheduleVM>> GetStudentPaymentSchedule(int studId)
    {
        //List<StudentPaymentScheduleVM> studentPaymentSchedules = new List<StudentPaymentScheduleVM>();
        List<StudentPaymentScheduleVM> finalPaymentScheduleVMs = new List<StudentPaymentScheduleVM>();
        try
        {
            var studentPaymentSchedules = await _context.StudentPaymentScheduleVMs.FromSqlInterpolated($"sp_get_payment_schedule_by_stuId {studId}").ToListAsync();

            var student = await _context.Student.FirstOrDefaultAsync(s => s.Id == studId);
            var admissionMonth = student.AdmissionDate.Date.Month;



            var existingFeeAllocations = await _context.StudentFeeAllocations.Where(s => s.UniqueId == student.UniqueId).ToListAsync();
            existingFeeAllocations = existingFeeAllocations.Where(s => s.IsActive == true).ToList();
            foreach (var item in studentPaymentSchedules)
            {
                if (item.SL>=1 && item.SL<=12)
                {
                    if (item.SL<admissionMonth)
                    {
                        continue;
                    }
                }
                var feeAllocation = existingFeeAllocations.FirstOrDefault(s => s.StudentFeeHeadId == item.FeeHeadId && s.ClassFeeListId == item.ClassFeeId);
                if (feeAllocation != null)
                {
                    item.Amount = feeAllocation.AllocatedAmount;
                }
                var admissionOrSession = student.AdmissionDate.Year < DateTime.Now.Year ? 13 : 0;
                if ((admissionOrSession == 13 && item.SL == 0) || (admissionOrSession == 0 && item.SL == 13))
                {
                    continue;
                }
                finalPaymentScheduleVMs.Add(item);
            }
        }
        catch (Exception)
        {
            throw;
        }

        return finalPaymentScheduleVMs;
    }
    public async Task<List<StudentPaymentSchedulePaidVM>> GetStudentPaymentSchedulePaid(int studId)
    {
        List<StudentPaymentSchedulePaidVM> studentPaymentSchedules = new List<StudentPaymentSchedulePaidVM>();
        try
        {
            studentPaymentSchedules = await _context.StudentPaymentSchedulePaidVMs.FromSqlInterpolated($"sp_get_scheduled_paid_by_id {studId}").ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
        return studentPaymentSchedules;
    }

    public async Task<IEnumerable<StudentPayment>> GetAllByStudentUniqueIdAsync(string uniqueId)
    {
        List<StudentPayment> payments = new List<StudentPayment>();
        try
        {
            payments = await _context.StudentPayment
            .Include(sp => sp.StudentPaymentDetails)
                .ThenInclude(sp => sp.StudentFeeHead)
            .Include(s => s.Student)
                .ThenInclude(ss => ss.AcademicClass)
            .Include(s => s.Student.AcademicSession)
            .Where(sp => sp.UniqueId == uniqueId).ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
        return payments;
    }
    public async Task<List<PaidAmountResult>> GetPaidAmountByFeeHead(string uniqueId, int sessionId, int isResidential, int classId, int feeHeadId)
    {
        List<PaidAmountResult> result;
        try
        {
            result = await _context.PaidAmountResults.FromSqlInterpolated($"EXEC sp_Get_PaidAmount {uniqueId}, {sessionId}, {isResidential}, {classId}, {feeHeadId}").ToListAsync();
        }
        catch (Exception)
        {

            throw;
        }
        return result;
    }

    public async Task<List<PreviouisPaymentDetailsDto>> GetAllStudentsPaymentSummeryAsync()
    {
        try
        {
            return await _context.PreviousPaymentsSummery
                .FromSqlRaw("EXEC sp_GetAllStudentsDueSummary")
                .ToListAsync();
        }
        catch (SqlException ex) when (ex.Number == 2812)
        {
            // Stored procedure not found - return empty list
            return new List<PreviouisPaymentDetailsDto>();
        }
    }

}
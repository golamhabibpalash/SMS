using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
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
                var existingFeeAllocations = await _context.StudentFeeAllocations.Where(s => s.UniqueId == student.UniqueId).ToListAsync();
                foreach (var item in studentPaymentSchedules)
                {
                    var feeAllocation = existingFeeAllocations.FirstOrDefault(s => s.StudentFeeHeadId == item.FeeHeadId);
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

        public async Task<double> GetStudentCurrentDue(int stuId)
        {
            double currentDue = 0.00;
            int currentMonth = DateTime.Now.Month;
            double totalCurrentPayable = 0;
            double totalCurrentPaid = 0;
            double admissionOrSessionFee = 0;
            double cMonthlyFee = 0;
            double othersFee = 0;
            var student = await _context.Student.FirstOrDefaultAsync(s => s.Id == stuId);
            var currentSession = await _context.AcademicSession.FirstOrDefaultAsync(s => s.CurrentSession == true);
            //Get Current Session Class Fee
            var classFees = await _context.ClassFeeList.Include(s => s.StudentFeeHead).Where(c => c.AcademicClassId == student.AcademicClassId && c.AcademicSessionId == currentSession.Id && c.StudentFeeHead.IsResidential == student.IsResidential).ToListAsync();

            var feeHeads = await _context.StudentFeeHead.Where(s => s.IsResidential == student.IsResidential).ToListAsync();
            var feeAllocations = await _context.StudentFeeAllocations.Where(s => s.UniqueId == student.UniqueId).ToListAsync();

            //0     = admission fee
            //1-12  = monthly fee
            //13    = session fee
            //14> =  other's fee

            //admission or session fee calculation
            var feeHeadSL = student.AdmissionDate.Year < DateTime.Now.Year ? 13 : 0;
            var feeHeadId = feeHeads.FirstOrDefault(s => s.SL == feeHeadSL).Id;
            admissionOrSessionFee = classFees.FirstOrDefault(s => s.SL == feeHeadSL)?.Amount ?? 0;
            var admissionOrSessionFeeAllocation = feeAllocations.FirstOrDefault(a => a.StudentFeeHeadId == feeHeadId);
            if (admissionOrSessionFeeAllocation != null)
            {
                admissionOrSessionFee = admissionOrSessionFeeAllocation.AllocatedAmount;
            }

            //monthly Fee Calculations
            int startingMonth = 1;
            if (student.AdmissionDate.Year == DateTime.Now.Year)
            {
                startingMonth = student.AdmissionDate.Month;
            }
            for (int i = startingMonth; i <= DateTime.Now.Month; i++)
            {
                feeHeadId = feeHeads.FirstOrDefault(s => s.SL == i).Id;
                var monthlyFeeAllocation = feeAllocations.FirstOrDefault(a => a.StudentFeeHeadId == feeHeadId);
                if (monthlyFeeAllocation != null)
                {
                    cMonthlyFee += monthlyFeeAllocation.AllocatedAmount;
                }
                else
                {
                    var cFees = classFees.FirstOrDefault(s => s.SL == i);
                    if (cFees != null)
                    {
                        cMonthlyFee += cFees.Amount;
                    }
                }
            }

            //Others Fee Calculations
            foreach (var item in classFees)
            {
                if (item.SL >= 14)
                {
                    var fHead = feeHeads.FirstOrDefault(s => s.SL == item.StudentFeeHead.SL);
                    if (fHead != null)
                    {
                        feeHeadId = fHead.Id;
                        var othersFeeAllocation = feeAllocations.FirstOrDefault(a => a.StudentFeeHeadId == feeHeadId);
                        othersFee += othersFeeAllocation != null ? othersFeeAllocation.AllocatedAmount : item.Amount;
                    }
                }
            }

            totalCurrentPayable = admissionOrSessionFee + cMonthlyFee + othersFee;

            var allPayments = await _context.StudentPayment.Where(s => s.StudentId == student.Id && s.AcademicSessionId == currentSession.Id).ToListAsync();
            totalCurrentPaid = allPayments.Sum(m => m.TotalPayment);

            currentDue = totalCurrentPayable - totalCurrentPaid;
            return currentDue;
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

    }
}

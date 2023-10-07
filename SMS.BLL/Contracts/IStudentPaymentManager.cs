using SMS.BLL.Contracts.Base;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IStudentPaymentManager : IManager<StudentPayment>
    {
        Task<IReadOnlyCollection<StudentPayment>> GetAllByStudentIdAsync(int id);
        Task<string> GetNewReceipt(int studentId, int feeHeadId);
        Task<IReadOnlyCollection<StudentPaymentSummeryVM>> GetPaymentSummeryByMonthYear(string monthYear);
        Task<IReadOnlyCollection<StudentPaymentSummeryVM>> GetPaymentSummeryByDate(string date); 
        Task<List<StudentPaymentScheduleVM>> GetStudentPaymentSchedule(int studId);
        Task<List<StudentPaymentSchedulePaidVM>> GetStudentPaymentSchedulePaid(int studId);
        Task<List<StudentPaymentSummerySMS_VM>> GetStudentPaymentSummerySMS_VMsAsync(DateTime date);
    }
}

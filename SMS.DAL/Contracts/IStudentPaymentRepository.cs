using SMS.DAL.Contracts.Base;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IStudentPaymentRepository : IRepository<StudentPayment>
    {
        Task<IReadOnlyCollection<StudentPayment>> GetAllByStudentIdAsync(int id);
        Task<List<StudentPaymentSummeryVM>> GetPaymentSummeryByMonthYear(string monthYear);
        Task<List<StudentPaymentSummeryVM>> GetPaymentSummeryByDate(string date);
    }
}

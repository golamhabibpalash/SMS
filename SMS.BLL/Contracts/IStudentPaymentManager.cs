using SMS.BLL.Contracts.Base;
using SMS.Entities;
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
    }
}

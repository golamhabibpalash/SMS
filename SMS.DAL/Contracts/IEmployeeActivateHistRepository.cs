using SMS.DAL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IEmployeeActivateHistRepository : IRepository<EmployeeActivateHist>
    {
        Task<bool> IsActiveByDateAsync(int id, string date);
    }
}

using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class EmployeeActivateHistRepository : Repository<EmployeeActivateHist>, IEmployeeActivateHistRepository
    {
        public EmployeeActivateHistRepository(ApplicationDbContext context) : base(context)
        {             
        }

        public async Task<bool> IsActiveByDateAsync(int id, string date)
        {
            List<EmployeeActivateHist> objListEmployeeActivateHist = new List<EmployeeActivateHist>();
            objListEmployeeActivateHist = (List<EmployeeActivateHist>)await GetAllAsync();
            objListEmployeeActivateHist = objListEmployeeActivateHist.Where(m => m.Id == id).ToList();
            return true;
        }
    }
}

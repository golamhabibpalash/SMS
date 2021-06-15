using SMS.DB;
using Microsoft.EntityFrameworkCore;
using SMS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMS.DAL.Contracts.Base;
using SMS.DAL.Repositories.Base;
using SMS.DAL.Contracts;

namespace Repositories
{

    public class EmployeeRepository :Repository<Employee>, IEmployeeRepository
    {
        private readonly ApplicationDbContext db;

        public EmployeeRepository(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }

        
    }
}

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

        public EmployeeRepository(ApplicationDbContext db):base(db)
        {
            
        }

        public override async Task<IReadOnlyCollection<Employee>> GetAllAsync()
        {
            return await _context.Employee
                .Include(e => e.Designation)
                .Include(e => e.PresentUpazila)
                .Include(e => e.PresentDistrict)
                .Include(e => e.PresentDivision)
                .Include(e => e.PermanentUpazila)
                .Include(e => e.PermanentDistrict)
                .Include(e => e.PermanentDivision)
                .Include(e => e.BloodGroup)
                .Include(e => e.Religion)
                .Include(e => e.Nationality)
                .Include(e => e.Gender)
                .ToListAsync();
        }

        public override async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employee
                .Include(e => e.Designation)
                .Include(e => e.PresentUpazila)
                .Include(e => e.PresentDistrict)
                .Include(e => e.PresentDivision)
                .Include(e => e.PermanentUpazila)
                .Include(e => e.PermanentDistrict)
                .Include(e => e.PermanentDivision)
                .Include(e => e.BloodGroup)
                .Include(e => e.Religion)
                .Include(e => e.Nationality)
                .Include(e => e.Gender)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee> GetByPhoneAttendance(string phoneLast9Digit)
        {
            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Phone.Substring(2,e.Phone.Length) == phoneLast9Digit);
            return employee;
        }
    }
}

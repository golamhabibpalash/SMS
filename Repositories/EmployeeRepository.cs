using DatabaseContext;
using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{

    public class EmployeeRepository : IEmployee
    {
        private readonly ApplicationDbContext db;

        public EmployeeRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public bool Add(Employee entity)
        {
            db.Employee.Add(entity);
            return db.SaveChanges() > 0;
        }

        public async Task<bool> AddAsync(Employee entity)
        {
            await db.Employee.AddAsync(entity);
            return await db.SaveChangesAsync() > 0;
        }

        public bool Delete(int id)
        {
            var emp = db.Employee.Find(id);
            db.Employee.Attach(emp);
            db.Employee.Remove(emp);
            return db.SaveChanges() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var emp =await db.Employee.FindAsync(id);
            db.Employee.Attach(emp);
            db.Employee.Remove(emp);
            return await db.SaveChangesAsync() > 0;
        }

        public IList<Employee> GetAll()
        {
            return db.Employee.ToList();
        }

        public async Task<ICollection<Employee>> GetAllAsync()
        {
            return await db.Employee.ToListAsync();
        }

        public Employee GetById(int id)
        {
            return db.Employee.Find(id);
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            var employee = await db.Employee
                .Include(e => e.Designation)
                .Include(e => e.EmpType)
                .Include(e => e.Gender)
                .Include(e => e.Nationality)
                .Include(e => e.PermanentDistrict)
                .Include(e => e.PermanentDivision)
                .Include(e => e.PermanentUpazila)
                .Include(e => e.PresentDistrict)
                .Include(e => e.PresentDivision)
                .Include(e => e.PresentUpazila)
                .Include(e => e.Religion)
                .FirstOrDefaultAsync(m => m.Id == id);
            return await db.Employee.FindAsync(id);
        }

        public bool Update(Employee entity)
        {
            db.Employee.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            return db.SaveChanges() > 0;
        }

        public async Task<bool> UpdateAsync(Employee entity)
        {
            db.Employee.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            return await db.SaveChangesAsync() > 0;
        }
    }
}

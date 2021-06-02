using DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class EmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> GetById(int id)
        {
            var emp =await _context.Employee.FirstOrDefaultAsync(e => e.Id == id);
            return emp;
        }

        public async Task<List<Employee>> GetAll()
        {
            return await _context.Employee.ToListAsync();
        }

        public async Task<bool> Add(Employee employee)
        {
            if (employee!=null)
            {
                await _context.Employee.AddAsync(employee);
                bool isSaved= _context.SaveChanges()>0;
                return isSaved;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> Update(Employee employee)
        {
            if (employee!=null)
            {
                _context.Entry(employee).State = EntityState.Modified;

                bool isSaved=await _context.SaveChangesAsync()>0;
                return isSaved;
            }
            else
            {
                return false;
            }
        }

    }
}

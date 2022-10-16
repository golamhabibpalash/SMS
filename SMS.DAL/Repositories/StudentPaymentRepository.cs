using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class StudentPaymentRepository : Repository<StudentPayment>, IStudentPaymentRepository
    {
        public StudentPaymentRepository(ApplicationDbContext db) : base(db)
        {

        }
        public override async Task<IReadOnlyCollection<StudentPayment>> GetAllAsync()
        {
            var payments = await _context
                .StudentPayment
                .Include(sp => sp.Student)
                .Include(sp => sp.StudentPaymentDetails)
                .ToListAsync();
            return payments;
        }
        public async Task<IReadOnlyCollection<StudentPayment>> GetAllByStudentIdAsync(int id)
        {
            return await _context.StudentPayment
                .Include(sp => sp.StudentPaymentDetails)
                .ThenInclude(sp => sp.StudentFeeHead)
                .Where(sp => sp.StudentId == id).ToListAsync();
        }

    
    }
}

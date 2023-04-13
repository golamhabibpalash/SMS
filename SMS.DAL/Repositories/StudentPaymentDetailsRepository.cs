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
    public class StudentPaymentDetailsRepository : Repository<StudentPaymentDetails>,IStudentPaymentDetailsRepository
    {
        private new readonly ApplicationDbContext _context;
        public StudentPaymentDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }
        public override async Task<IReadOnlyCollection<StudentPaymentDetails>> GetAllAsync()
        {
            List<StudentPaymentDetails> list = await _context.StudentPaymentDetails.Include(s => s.StudentFeeHead).Include(s => s.StudentPayment).ToListAsync();
            return list;
        }

        public async Task<List<StudentPaymentDetails>> GetAllByPaymentId(int studentPaymentId)
        {
            List<StudentPaymentDetails> studentPaymentDetails = await _context.StudentPaymentDetails.Where(s => s.StudentPaymentId == studentPaymentId).ToListAsync();
            return studentPaymentDetails;
        }

        public override async Task<StudentPaymentDetails> GetByIdAsync(int id)
        {
            try
            {
                StudentPaymentDetails studentPaymentDetails = await _context.StudentPaymentDetails
                    .Include(s => s.StudentFeeHead)
                    .Include(s => s.StudentPayment)
                    .FirstOrDefaultAsync(m => m.Id == id);
                return studentPaymentDetails;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

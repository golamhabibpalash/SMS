using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
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
            List<StudentPayment> payments = new List<StudentPayment>();
            try
            {
                payments = await _context
                .StudentPayment
                .Include(sp => sp.Student)
                .Include(sp => sp.StudentPaymentDetails)
                .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            
            return payments;
        }
        public async Task<IReadOnlyCollection<StudentPayment>> GetAllByStudentIdAsync(int id)
        {
            List<StudentPayment> payments = new List<StudentPayment>();
            try
            {
                payments = await _context.StudentPayment
                .Include(sp => sp.StudentPaymentDetails)
                .ThenInclude(sp => sp.StudentFeeHead)
                .Where(sp => sp.StudentId == id).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return payments;
        }

        public async Task<List<StudentPaymentSummeryVM>> GetPaymentSummeryByDate(string date)
        {
            List<StudentPaymentSummeryVM> payments = new List<StudentPaymentSummeryVM>();
            try
            {
                payments = await _context.StudentPaymentSummeryVMs.FromSqlInterpolated($"sp_get_payWithClass_by_date {date}").ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            
           
            return payments;
        }



        public async Task<List<StudentPaymentSummeryVM>> GetPaymentSummeryByMonthYear(string monthYear)
        {
            List<StudentPaymentSummeryVM> payments = new List<StudentPaymentSummeryVM>();
            try
            {
                payments = await _context.StudentPaymentSummeryVMs.FromSqlInterpolated($"sp_get_payWithClass_by_monthyear {monthYear}").ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            
            return payments;
        }
    }
}

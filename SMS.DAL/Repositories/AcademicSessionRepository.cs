using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.DAL.Repositories.Base;
using SMS.DAL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SMS.DAL.Repositories
{
    public class AcademicSessionRepository:Repository<AcademicSession>, IAcademicSessionRepository
    {
        private new readonly ApplicationDbContext _context;
        public AcademicSessionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<AcademicSession> GetCurrentAcademicSession()
        {
            AcademicSession currentSession = new();
            try
            {
                currentSession =await _context.AcademicSession.FirstOrDefaultAsync(s => s.CurrentSession == true);
            }
            catch (Exception)
            {
                throw;
            }
            return currentSession;
        }
    }
}

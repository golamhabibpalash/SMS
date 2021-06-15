using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.DAL.Repositories.Base;
using SMS.DAL.Contracts;

namespace SMS.DAL.Repositories
{
    public class AcademicSessionRepository:Repository<AcademicSession>, IAcademicSessionRepository
    {
        private readonly ApplicationDbContext _context;
        public AcademicSessionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

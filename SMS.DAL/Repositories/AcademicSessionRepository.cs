using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.DAL.Repositories.Base;

namespace Repositories
{
    public class AcademicSessionRepository:Repository<AcademicSession>
    {
        private readonly ApplicationDbContext _context;
        public AcademicSessionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

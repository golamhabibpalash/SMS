using DatabaseContext;
using Models;
using Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

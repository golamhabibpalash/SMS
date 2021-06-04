using DatabaseContext;
using Models;
using Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class StudentRepository : Repository<Student>
    {
        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}

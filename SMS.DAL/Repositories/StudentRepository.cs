using SMS.DB;
using SMS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SMS.DAL.Repositories.Base;

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

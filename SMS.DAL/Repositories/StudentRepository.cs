using SMS.DB;
using SMS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SMS.DAL.Repositories.Base;
using SMS.DAL.Contracts;

namespace SMS.DAL.Repositories
{
    public class StudentRepository : Repository<Student>,IStudentRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }


    }
}

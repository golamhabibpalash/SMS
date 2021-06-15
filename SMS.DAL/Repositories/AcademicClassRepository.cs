using SMS.Entities;
using SMS.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SMS.DAL.Repositories.Base;

namespace SMS.DAL
{
    public class AcademicClassRepository:Repository<AcademicClass>
    {
        private readonly ApplicationDbContext _db;

        public AcademicClassRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        //GetAllBySessionIdAsync
        public async Task<ICollection<AcademicClass>> GetAllBySessionIdAsync(int id)
        {
            return await _db.AcademicClass.Where(a => a.Id == id).ToListAsync();
        }
    }
}

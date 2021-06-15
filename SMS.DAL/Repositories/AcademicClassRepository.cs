using SMS.Entities;
using SMS.DB;
using SMS.DAL.Repositories.Base;
using SMS.DAL.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace SMS.DAL.Repositories
{
    public class AcademicClassRepository:Repository<AcademicClass>, IAcademicClassRepository
    {
        private readonly ApplicationDbContext _db;

        public AcademicClassRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        
    }
}

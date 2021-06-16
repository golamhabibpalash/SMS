using SMS.DAL.Contracts;
using SMS.DAL.Contracts.Base;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class AcademicSectionRepository : Repository<AcademicSection>, IAcademicSectionRepositoy 
    {
        public AcademicSectionRepository(ApplicationDbContext db):base(db)
        {

        }
    }
}

using SMS.DAL.Repositories.Base;
using SMS.DAL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.DB;

namespace SMS.DAL.Repositories
{
    public class NationalityRepository : Repository<Nationality>, INationalityRepository
    {
        public NationalityRepository(ApplicationDbContext db):base(db)
        {

        }
    }
}

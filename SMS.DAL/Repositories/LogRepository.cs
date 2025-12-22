using System;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;

namespace SMS.DAL.Repositories;

public class LogRepository : Repository<Log>
{
    public LogRepository(ApplicationDbContext db) : base(db)
    {
        
    }
}

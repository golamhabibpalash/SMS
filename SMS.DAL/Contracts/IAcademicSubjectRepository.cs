using SMS.DAL.Contracts.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IAcademicSubjectRepository:IRepository<AcademicSubject>
    {
        Task<IEnumerable<AcademicSubject>> GetSubjectsByClassIdAsync(int classId);
    }
}

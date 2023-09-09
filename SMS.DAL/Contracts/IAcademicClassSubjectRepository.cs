using SMS.DAL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IAcademicClassSubjectRepository:IRepository<AcademicClassSubject>
    {
        Task<List<AcademicSubject>> GetSubjectsByClassIdAsync(int classId);
    }
}

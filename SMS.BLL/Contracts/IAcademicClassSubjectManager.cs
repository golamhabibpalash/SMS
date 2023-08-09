using SMS.BLL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IAcademicClassSubjectManager : IManager<AcademicClassSubject>
    {
        Task<List<AcademicSubject>> GetSubjectsByClassIdAsync(int classId);
    }
}

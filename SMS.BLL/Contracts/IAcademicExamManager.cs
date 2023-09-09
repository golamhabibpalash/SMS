using SMS.BLL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IAcademicExamManager:IManager<AcademicExam>
    {
        Task<List<AcademicExam>> GetByClassIdExamGroupId(int examGroupId, int academicClassId);
    }
}

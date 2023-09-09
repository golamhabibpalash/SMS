using SMS.DAL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IAcademicExamRepository:IRepository<AcademicExam>
    {
        Task<List<AcademicExam>> GetByClassIdExamGroupId(int examGroupId, int academicClassId);
    }
}

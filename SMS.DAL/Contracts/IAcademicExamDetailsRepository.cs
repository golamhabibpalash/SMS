using SMS.DAL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IAcademicExamDetailsRepository : IRepository<AcademicExamDetail>
    {
        Task<List<AcademicExamDetail>> GetByExamIdAsync(int examId);
        Task<List<AcademicExamDetail>> GetAllByExamGroupAndStudentId(int examGroupId, int studentId);
    }    
}

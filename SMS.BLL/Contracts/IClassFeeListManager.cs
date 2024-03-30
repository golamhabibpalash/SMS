using SMS.BLL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IClassFeeListManager : IManager<ClassFeeList>
    {
        Task<ClassFeeList> GetByClassIdAndFeeHeadIdAsync(int classId, int feeHeadId, int sessionId);
        Task<List<ClassFeeList>> GetClassFeeListByClassIdFeeHeadIdSessionIdAsync(int classId, int feeHeadId, int sessionId);
        Task<List<ClassFeeList>> GetAllByClassIdAsync(int classId);
        Task<double>GetFeeAmountByFeeListSlAsync(string uniquId, int sl);
        Task<List<ClassFeeList>> GetByClassIdSessionIdAsync(int classId, int sessionId);
        Task<List<ClassFeeList>> GetByClassIdSessionIdStudentIdAsync(int classId, int sessionId, int studentId);
    }
}

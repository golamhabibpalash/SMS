using SMS.DAL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IClassFeeListRepository : IRepository<ClassFeeList>
    {
        Task<ClassFeeList> GetByClassIdAndFeeHeadIdAsync(int classId, int feeHeadId, int sessionId);
        Task<List<ClassFeeList>> GetAllByClassIdAsync(int classId);
        Task<List<ClassFeeList>> GetClassFeeListByClassIdFeeHeadIdSessionIdAsync(int classId, int feeHeadId, int sessionId);
        Task<double> GetFeeAmountByFeeListSL(string uniquId, int sl);
    }
}

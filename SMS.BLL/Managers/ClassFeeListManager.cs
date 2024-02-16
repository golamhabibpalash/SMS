using BLL.Managers.Base;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class ClassFeeListManager : Manager<ClassFeeList>, IClassFeeListManager
    {
        private readonly IClassFeeListRepository _classFeeListRepository;
        public ClassFeeListManager(IClassFeeListRepository classFeeListRepository) :base(classFeeListRepository)
        {
            _classFeeListRepository = classFeeListRepository;
        }

        public async Task<List<ClassFeeList>> GetAllByClassIdAsync(int classId)
        {
            return await _classFeeListRepository.GetAllByClassIdAsync(classId);
        }

        public async Task<ClassFeeList> GetByClassIdAndFeeHeadIdAsync(int classId, int feeHeadId, int sessionId)
        {
            var result = await _classFeeListRepository.GetByClassIdAndFeeHeadIdAsync(classId, feeHeadId,sessionId);
            return result;
        }

        public async Task<List<ClassFeeList>> GetByClassIdSessionIdAsync(int classId, int sessionId)
        {
            var result = await _classFeeListRepository
                .Table
                .Where(s => s.AcademicClassId == classId &&
                    s.AcademicSessionId == sessionId)
                .ToListAsync();
            return result;
        }

        public async Task<List<ClassFeeList>> GetClassFeeListByClassIdFeeHeadIdSessionIdAsync(int classId, int feeHeadId, int sessionId)
        {
            List<ClassFeeList> result = await _classFeeListRepository.GetClassFeeListByClassIdFeeHeadIdSessionIdAsync(classId, feeHeadId, sessionId);
            return result;
        }

        public async Task<double> GetFeeAmountByFeeListSlAsync(string uniquId, int sl)
        {
            var result = await _classFeeListRepository.GetFeeAmountByFeeListSL(uniquId, sl);
            return result;
        }
    }
}

using BLL.Managers.Base;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class ClassFeeListManager : Manager<ClassFeeList>, IClassFeeListManager
    {
        private readonly IClassFeeListRepository _classFeeListRepository;
        private readonly IStudentManager studentManager;

        public ClassFeeListManager(IClassFeeListRepository classFeeListRepository, IStudentManager studentManager) :base(classFeeListRepository)
        {
            _classFeeListRepository = classFeeListRepository;
            this.studentManager = studentManager;
        }

        public async Task<List<ClassFeeList>> GetAllByClassIdAsync(int classId)
        {
            return await _classFeeListRepository.GetAllByClassIdAsync(classId);
        }

        public async Task<List<ClassFeeList>> GetAllByStudentId(int studId)
        {
            Student student = await studentManager.GetByIdAsync(studId);
            List<ClassFeeList> classFeeLists =await _classFeeListRepository.Table.Where(s => s.AcademicClassId == student.AcademicClassId && s.AcademicSessionId == student.AcademicSessionId && s.StudentFeeHead.IsResidential == student.IsResidential).ToListAsync();
            return classFeeLists;
        }

        public async Task<ClassFeeList> GetByClassIdAndFeeHeadIdAsync(int classId, int feeHeadId, int sessionId)
        {
            var result = await _classFeeListRepository.GetByClassIdAndFeeHeadIdAsync(classId, feeHeadId,sessionId);
            return result;
        }

        public async Task<List<ClassFeeList>> GetClassFeeListByClassIdFeeHeadIdSessionIdAsync(int classId, int feeHeadId, int sessionId)
        {
            List<ClassFeeList> result = await _classFeeListRepository.GetClassFeeListByClassIdFeeHeadIdSessionIdAsync(classId, feeHeadId, sessionId);
            return result;
        }
    }
}

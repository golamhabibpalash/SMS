using BLL.Managers.Base;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.BLL.Managers;

public class ClassFeeListManager : Manager<ClassFeeList>, IClassFeeListManager
{
    private readonly IClassFeeListRepository _classFeeListRepository;
    private readonly IStudentManager _studentManager;
    private readonly IAcademicSessionManager _sessionManager;

    public ClassFeeListManager(IClassFeeListRepository classFeeListRepository, IStudentManager studentManager, IAcademicSessionManager sessionManager) : base(classFeeListRepository)
    {
        _classFeeListRepository = classFeeListRepository;
        _studentManager = studentManager;
        _sessionManager = sessionManager;
    }

    public async Task<List<ClassFeeList>> GetAllByClassIdAsync(int classId)
    {
        return await _classFeeListRepository.GetAllByClassIdAsync(classId);
    }

    public async Task<List<ClassFeeList>> GetAllByStudentId(int studId)
    {
        Student student = await _studentManager.GetByIdAsync(studId);
        List<ClassFeeList> classFeeLists =await _classFeeListRepository.Table.Where(s => s.AcademicClassId == student.AcademicClassId && s.AcademicSessionId == student.AcademicSessionId && s.StudentFeeHead.IsResidential == student.IsResidential).ToListAsync();
        return classFeeLists;
    }

    public async Task<ClassFeeList> GetByClassIdAndFeeHeadIdAsync(int classId, int feeHeadId, int sessionId)
    {
        var result = await _classFeeListRepository.GetByClassIdAndFeeHeadIdAsync(classId, feeHeadId,sessionId);
        return result;
    }

    public async Task<List<ClassFeeList>> GetByClassIdSessionIdStudentIdAsync(int classId, int sessionId, int studentId)
    {
        var result = await _classFeeListRepository.GetByClassIdSessionIdStudentIdAsync(classId, sessionId, studentId);
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
    public async Task<List<ClassFeeList>> GetClassFeeByUniquId(string uniquId)
    {
        var result = await _classFeeListRepository.GetCurrentAllByUniqueId(uniquId);
        return result;
    }

    public async Task<List<ClassFeeList>> GetAllBySessionClassTypeAsync(int sessionId, int classId, bool isResidential)
    {
        var allListByClass = await _classFeeListRepository.GetAllBySessionIdClassIdAsync(sessionId, classId);
        var result = allListByClass.Where(s => s.StudentFeeHead.IsResidential == isResidential).ToList();
        return result;
    }    
}

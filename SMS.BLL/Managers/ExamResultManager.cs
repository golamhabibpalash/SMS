using BLL.Managers.Base;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers;

public class ExamResultManager : Manager<ExamResult>, IExamResultManager
{
    private readonly IExamResultRepository _examResultRepository;
    public ExamResultManager(IExamResultRepository examResultRepository):base(examResultRepository)
    {
        _examResultRepository = examResultRepository;
    }

    public async Task<List<ExamResult>> GetExamResultsByExamGroupNClassId(int examGroupId, int classId)
    {
        return await _examResultRepository.GetExamResultsByExamGroupNClassId(examGroupId, classId);
    }

    public async Task<string> GetHighestMarksOfTheClassAsync(int examGroupId, int classId)
    {
        var maxMarks = await _examResultRepository.Table
                    .Where(e => e.AcademicClassId == classId && e.AcademicExamGroupId == examGroupId)
                    .MaxAsync(e => (int?)e.TotalObtainMarks) ?? 0;
        return maxMarks.ToString();
    }

    public bool IsResultProcessedAsync(int examGroupId, int classId)
    {
        return _examResultRepository.IsResultProcessedAsync(examGroupId, classId);
    }

    public async Task<Dictionary<(int ExamGroupId, int ClassId), bool>> GetResultProcessedStatusBulkAsync(IEnumerable<(int ExamGroupId, int ClassId)> examGroupAndClassPairs)
    {
        return await _examResultRepository.GetResultProcessedStatusBulkAsync(examGroupAndClassPairs);
    }
}

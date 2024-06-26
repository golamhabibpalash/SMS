﻿using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class ExamResultManager:Manager<ExamResult>,IExamResultManager
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

        public bool IsResultProcessedAsync(int examGroupId, int classId)
        {
            return _examResultRepository.IsResultProcessedAsync(examGroupId, classId);
        }
    }
}

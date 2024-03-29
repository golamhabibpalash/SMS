﻿using SMS.DAL.Contracts.Base;
using SMS.DAL.Repositories;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IAcademicExamGroupRepository:IRepository<AcademicExamGroup>
    {
        Task<IReadOnlyCollection<AcademicExamGroup>> GetByMonthExamType(int monthId, int examTypeId);
    }
}

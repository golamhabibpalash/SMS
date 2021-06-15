﻿using SMS.BLL.Contracts.Base;
using SMS.Entities;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IAcademicClassManager : IManager<AcademicClass>
    {
        Task<bool> GetByName(string entityName);
    }
}

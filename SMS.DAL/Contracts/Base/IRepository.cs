﻿using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts.Base
{
    public interface IRepository<T> where T:class
    {
        T GetById(int id);

        Task<T> GetByIdAsync(int id);

        Task<IReadOnlyCollection<T>> GetAllAsync();

        Task<bool> AddAsync(T entity);

        Task<bool> UpdateAsync(T entity);

        Task<bool> RemoveAsync(T entity);

        Task<bool> IsExistByIdAsync(int id);

        Task<bool> IsExistAsync(T entity);

        Task<bool> SaveAfterAddAsync();
        bool AddWithoutSave(T entity);
        IQueryable<T> Table { get; }
    }
}

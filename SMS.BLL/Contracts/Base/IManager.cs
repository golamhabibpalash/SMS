﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts.Base
{
    public interface IManager<T> where T:class
    {
        T GetById(int id);
        public Task<T> GetByIdAsync(int id);

        Task<IReadOnlyCollection<T>> GetAllAsync();

        Task<bool> AddAsync(T entity);

        Task<bool> UpdateAsync(T entity);

        Task<bool> RemoveAsync(T entity);

        Task<bool> IsExistByIdAsync(int id);

        Task<bool> IsExistAsync(T entity);

        Task<bool> SaveAfterAddAsync();
        bool AddWithoutSave(T entity);
    }
}

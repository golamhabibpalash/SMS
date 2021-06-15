using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts.Base
{
    public interface IRepository<T> where T:class
    {
        Task<T> GetByIdAsync(int id);

        Task<IReadOnlyCollection<T>> GetAllAsync();

        Task<bool> AddAsync(T entity);

        Task<bool> UpdateAsync(T entity);

        Task<bool> RemoveAsync(T entity);

        Task<bool> IsExistByIdAsync(int id);
    }
}

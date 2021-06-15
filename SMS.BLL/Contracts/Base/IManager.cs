using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts.Base
{
    public interface IManager<T> where T:class
    {
        public Task<T> GetByIdAsync(int id);

        Task<IReadOnlyCollection<T>> GetAllAsync();

        Task<bool> AddAsync(T entity);

        Task<bool> UpdateAsync(T entity);

        Task<bool> RemoveAsync(T entity);

        Task<bool> IsExistByIdAsync(int id);
    }
}

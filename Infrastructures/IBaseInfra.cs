using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures
{
    public interface IBaseInfra<T> where T:class
    {
        T GetById(int id);
        Task<T> GetByIdAsync(int id);

        IList<T> GetAll();
        Task<ICollection<T>> GetAllAsync();

        bool Add(T entity);
        Task<bool> AddAsync(T entity);

        bool Update(T entity);
        Task<bool> UpdateAsync(T entity);

        bool Delete(int id);
        Task<bool> DeleteAsync(int id);
    }
}

using SMS.BLL.Contracts.Base;
using SMS.DAL.Contracts.Base;
using SMS.DAL.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Managers.Base
{
    public abstract class Manager<T>:IManager<T> where T:class
    {
        private readonly IRepository<T> _repository;
        public Manager(IRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            return await _repository.AddAsync(entity);
        }

        public virtual async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<bool> RemoveAsync(T entity)
        {
            return await _repository.RemoveAsync(entity);
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<bool> IsExistByIdAsync(int id)
        {
            return await _repository.IsExistByIdAsync(id);
        }

}

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

        public async Task<bool> AddAsync(T entity)
        {
            return await _repository.AddAsync(entity);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> RemoveAsync(T entity)
        {
            return await _repository.RemoveAsync(entity);
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            return await _repository.UpdateAsync(entity);
        }
    }
}

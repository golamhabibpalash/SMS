using DatabaseContext;
using Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Base
{
    public abstract class Manager<T> where T:class
    {
        private readonly Repository<T> _repository;
        public Manager(Repository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<bool> Add(T entity)
        {
            return await _repository.Add(entity);
        }
    }
}

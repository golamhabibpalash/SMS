using SMS.DB;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMS.DAL.Contracts.Base;
using System;

namespace SMS.DAL.Repositories.Base
{
    public abstract class Repository<T> : IRepository<T> where T:class
    {
        protected readonly ApplicationDbContext _context;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }
        public DbSet<T> Table 
        {
            get { return _context.Set<T>(); } 
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await Table.FindAsync(id);
        }

        public virtual async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await Table.ToListAsync();
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            Table.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> RemoveAsync(T entity)
        {
            Table.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> IsExistByIdAsync(int id)
        {
            var result = await Table.FindAsync(id);
            if (result!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

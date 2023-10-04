using SMS.DB;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMS.DAL.Contracts.Base;
using System;
using SMS.Entities;

namespace SMS.DAL.Repositories.Base
{
    public abstract class Repository<T> : Contracts.Base.IRepository<T> where T:class
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
            await Table.AddAsync(entity);
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

        public virtual async Task<bool> IsExistAsync(T entity)
        {
            await Table.FindAsync(entity);
            return true;
        }

        public T GetById(int id)
        {
            return Table.Find(id);
        }

        public async Task<bool> SaveAfterAddAsync()
        {
            try
            {
                bool isSaved = await _context.SaveChangesAsync() > 0;
                if (isSaved)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public bool AddWithoutSave(T entity)
        {
            try
            {
                Table.Add(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

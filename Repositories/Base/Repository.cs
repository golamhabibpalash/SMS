using DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Base
{
    public abstract class Repository<T> where T:class
    {
        private readonly ApplicationDbContext _context;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }
        //private ApplicationDbContext _context = new ApplicationDbContext();
        public DbSet<T> Table 
        {
            get { return _context.Set<T>(); } 
        }

        public virtual async Task<T> GetById(int id)
        {
            return await Table.FindAsync(id);
        }

        public virtual async Task<ICollection<T>> GetAll()
        {
            return await Table.ToListAsync();
        }

        public virtual async Task<bool> Add(T entity)
        {
            Table.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }

        public virtual async Task<bool> Remove(T entity)
        {
            Table.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

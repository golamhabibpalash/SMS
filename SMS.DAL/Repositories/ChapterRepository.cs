using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class ChapterRepository : Repository<Chapter>, IChapterRepository
    {
        //private readonly ApplicationDbContext _context;
        public ChapterRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        public override async Task<IReadOnlyCollection<Chapter>> GetAllAsync()
        {
            var chapters = await _context.Chapters
                .Include(c => c.AcademicSubject)
                    .ThenInclude(d => d.AcademicClass).ToListAsync();
            return chapters;
        }

        public override async Task<Chapter> GetByIdAsync(int id)
        {
            var chapter = await _context.Chapters
                .Include(a => a.AcademicSubject)
                    .ThenInclude(c => c.AcademicClass)
                .FirstOrDefaultAsync(m => m.Id == id);
            return chapter;
        }
    }
}

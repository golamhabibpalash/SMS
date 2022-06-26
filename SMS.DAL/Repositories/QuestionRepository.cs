using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Contracts.Base;
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
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        public QuestionRepository(ApplicationDbContext db) : base(db)
        {

        }

        public override async Task<IReadOnlyCollection<Question>> GetAllAsync()
        {
            var questions = await _context.Questions
                .Include(q => q.QuestionDetails)
                .Include(q => q.Chapter)
                    .ThenInclude(c => c.AcademicSubject)
                        .ThenInclude(d => d.AcademicClass)
                .ToListAsync();
            return questions;
        }
    }
}

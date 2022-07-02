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
    public class QuestionFormationRepository : Repository<QuestionFormat>, IQuestionFormationRepository
    {
        public QuestionFormationRepository(ApplicationDbContext db):base(db)
        {
        }

        public async Task<QuestionFormat> GetQuestionFormatByFormationAsync(string formation)
        {
            var existingFormation = await _context.QuestionFormats.Where(m => m.QFormat.Trim() == formation.Trim()).FirstOrDefaultAsync();
            return existingFormation;
            
        }

        public async Task<QuestionFormat> GetQuestionFormatByNameAsync(string formationName)
        {
            var existingFormation = await _context.QuestionFormats.Where(m => m.Name.Trim() == formationName.Trim()).FirstOrDefaultAsync();
            return existingFormation;
        }
    }
}

using SMS.DAL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IQuestionFormationRepository : IRepository<QuestionFormat>
    {
        Task<QuestionFormat> GetQuestionFormatByNameAsync(string formationName);
        Task<QuestionFormat> GetQuestionFormatByFormationAsync(string formation);
    }
}

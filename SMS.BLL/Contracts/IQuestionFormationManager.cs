using SMS.BLL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IQuestionFormationManager : IManager<QuestionFormat>
    {
        Task<bool> GetQuestionFormatByNameAsync(string formationName);
        Task<bool> GetQuestionFormatByFormationAsync(string formation);
    }
}

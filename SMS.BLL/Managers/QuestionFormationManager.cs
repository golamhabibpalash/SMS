using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class QuestionFormationManager : Manager<QuestionFormat>, IQuestionFormationManager
    {
        private readonly IQuestionFormationRepository _questionFormationRepository;
        public QuestionFormationManager(IQuestionFormationRepository repo):base(repo)
        {
            _questionFormationRepository = repo;
        }

        public async Task<bool> GetQuestionFormatByFormationAsync(string formation)
        {
            var extingFormation = await _questionFormationRepository.GetQuestionFormatByFormationAsync(formation);
            if (extingFormation!=null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> GetQuestionFormatByNameAsync(string formationName)
        {
            var extingFormation = await _questionFormationRepository.GetQuestionFormatByNameAsync(formationName);
            if (extingFormation != null)
            {
                return true;
            }
            return false;
        }
    }
}

using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class QuestionManager : Manager<Question>, IQuestionManager
    {
        public QuestionManager(IQuestionRepository repository):base(repository)
        {

        }
    }
}

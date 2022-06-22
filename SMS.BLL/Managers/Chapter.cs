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
    public class ChapterManager : Manager<Chapter>, IChapterManager
    {
        private readonly IChapterRepository _repository;
        public ChapterManager(IChapterRepository repository) : base(repository)
        {
            _repository = repository;
        }
    }
}

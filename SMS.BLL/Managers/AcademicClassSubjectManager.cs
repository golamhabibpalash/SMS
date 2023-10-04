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
    public class AcademicClassSubjectManager:Manager<AcademicClassSubject>, IAcademicClassSubjectManager
    {
        private readonly IAcademicClassSubjectRepository _classSubjectRepository;
        public AcademicClassSubjectManager(IAcademicClassSubjectRepository repository):base(repository)
        {
            _classSubjectRepository = repository;
        }

        public async Task<List<AcademicSubject>> GetSubjectsByClassIdAsync(int classId)
        {
            try
            {
                var allSubjects = await _classSubjectRepository.GetSubjectsByClassIdAsync(classId);
                return allSubjects;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}

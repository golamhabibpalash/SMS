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
        public AcademicClassSubjectManager(IAcademicClassSubjectRepository _repository):base(_repository)
        {
            
        }

        public async Task<List<AcademicSubject>> GetSubjectsByClassIdAsync(int classId)
        {
            try
            {
                var allSubjects = await _repository.GetAllAsync();
                List<AcademicSubject> subjects =(from s in allSubjects
                                                where s.AcademicClassId == classId
                                                select s.AcademicSubject).ToList();

                return subjects;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}

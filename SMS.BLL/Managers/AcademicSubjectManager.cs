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
    public class AcademicSubjectManager : Manager<AcademicSubject>,IAcademicSubjectManager
    {
        private readonly IAcademicSubjectRepository _academicSubjectRepository;
        public AcademicSubjectManager(IAcademicSubjectRepository academicSubjectRepository):base(academicSubjectRepository)
        {
            _academicSubjectRepository = academicSubjectRepository;
        }
        public override async Task<bool> AddAsync(AcademicSubject entity)
        {
            bool isExist = await _repository.IsExistAsync(entity);

            if (isExist)
            {
                return false;
            }
            else
            {
              return await _repository.AddAsync(entity);
            }
        }
        
    }
}

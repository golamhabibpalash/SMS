using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class AcademicClassManager:Manager<AcademicClass>, IAcademicClassManager
    {
        private readonly IAcademicClassRepository _academicClassRepository;
        public AcademicClassManager(IAcademicClassRepository academicClassRepository):base(academicClassRepository)
        {
            _academicClassRepository = academicClassRepository;
        }

        public async Task<bool> GetByName(string entityName)
        {
            var aClass = await _academicClassRepository.GetByNameAsync(entityName.Trim());
            if (aClass!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override async Task<bool> AddAsync(AcademicClass entity)
        {
            var isExist = await GetByName(entity.Name);
            if (isExist)
            {
                return false;
            }
            else
            {
                return await base.AddAsync(entity);
            }
        }
        public override async Task<bool> UpdateAsync(AcademicClass entity)
        {
            var existClass = await _academicClassRepository.GetByNameAsync(entity.Name);
            if (existClass != null)
            {
                if (entity.Id!=existClass.Id)
                {
                    return false;
                }

            }
            return await _academicClassRepository.UpdateAsync(entity);
        }
    }
}

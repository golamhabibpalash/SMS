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
    public class AcademicSectionManager : Manager<AcademicSection>, IAcademicSectionManager
    {
        private readonly IAcademicSectionRepositoy _academicSectinRepository;
        public AcademicSectionManager(IAcademicSectionRepositoy academicSectionRepository):base(academicSectionRepository)
        {
            _academicSectinRepository = academicSectionRepository;
        }

        public async Task<IReadOnlyCollection<AcademicSection>> GetAllByClassWithSessionId(int classId, int sessionId)
        {
            return await _academicSectinRepository.GetAllByClassWithSessionId(classId, sessionId);
        }
    }
}

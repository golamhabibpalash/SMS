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
    public class DistrictManager : Manager<District>, IDistrictManager
    {
        private readonly IDistrictRepository _districtRepository;

        public DistrictManager(IDistrictRepository districtRepository) : base(districtRepository)
        {
            _districtRepository = districtRepository;
        }

        Task<IReadOnlyCollection<District>> IDistrictManager.GetAllByDivId(int id)
        {
            return _districtRepository.GetAllByDivId(id);
        }
    }
}

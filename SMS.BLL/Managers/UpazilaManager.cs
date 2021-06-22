using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class UpazilaManager : Manager<Upazila>, IUpazilaManager
    {
        private readonly IUpazilaRepository _upazilaRepository;
        public UpazilaManager(IUpazilaRepository upazilaRepository) : base(upazilaRepository)
        {
            _upazilaRepository = upazilaRepository;
        }

        public async Task<IReadOnlyCollection<Upazila>> GetByDistrictId(int id)
        {
            return await _upazilaRepository.GetByDistrictId(id);
        }
    }
}

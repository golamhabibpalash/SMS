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
    public class ClaimStoreManager:Manager<ClaimStores>, IClaimStoreManager
    {
        private readonly IClaimStoreRepository _claimStoreRepository;
        public ClaimStoreManager(IClaimStoreRepository claimStoreRepository):base(claimStoreRepository)
        {
            _claimStoreRepository = claimStoreRepository;
        }

        public async Task<bool> IsExistAsync(string claimValue, int subModuleId)
        {
            var cStores = await _claimStoreRepository.GetAllAsync();
            var cStore = cStores.FirstOrDefault(s => s.ClaimValue == claimValue && s.SubModuleId == subModuleId);

            return cStore != null?true:false;
        }
    }
}

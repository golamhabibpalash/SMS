using SMS.BLL.Contracts.Base;
using SMS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IAppliedStudentManager : IManager<AppliedStudent>
    {
        Task<List<AppliedStudent>> SearchBySearchText(string searchText);
    }
}

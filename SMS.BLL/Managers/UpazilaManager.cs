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
        public UpazilaManager(IUpazilaRepository upazilaRepository) : base(upazilaRepository)
        {

        }
    }
}

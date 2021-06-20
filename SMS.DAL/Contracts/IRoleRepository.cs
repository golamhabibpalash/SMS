using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SMS.DAL.Contracts.Base;
using SMS.Entities;
namespace SMS.DAL.Contracts
{
    public interface IRoleRepository : IRepository<RoleManager<IdentityRole>>
    {
    }
}

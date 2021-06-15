using BLL.Managers.Base;
using Repositories;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;

namespace SMS.BLL.Managers
{
    public class EmployeeManager:Manager<Employee>, IEmployeeManager
    {
        public EmployeeManager(IEmployeeRepository employeeRepository):base(employeeRepository)
        {

        }
    }
}

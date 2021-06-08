using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures
{
    public interface IEmployee : IBaseInfra<Employee>
    {
        //Employee GetById(int id);
        //Task<Employee> GetByIdAsync(int id);

        //IList<Employee> GetAll();
        //Task<ICollection<Employee>> GetAllAsync();

        //bool Add(Employee employee);
        //Task<bool> AddAsync(Employee employee);

        //bool Update(Employee employee);
        //Task<bool> UpdateAsync(Employee employee);

        //bool Delete(Employee employee);
        //Task<bool> DeleteAsync(Employee employee);
    }
}

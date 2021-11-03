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
    public class StudentFeeHeadManager : Manager<StudentFeeHead>, IStudentFeeHeadManager
    {
        private readonly IStudentFeeHeadRepository _studentFeeHeadRepository;

        public StudentFeeHeadManager(IStudentFeeHeadRepository studentFeeHeadRepository):base(studentFeeHeadRepository)
        {
            _studentFeeHeadRepository = studentFeeHeadRepository;
        }

        public async Task<StudentFeeHead> GetByNameAsync(string name)
        {
            return await _studentFeeHeadRepository.GetByNameAsync(name);
        }
    }
}

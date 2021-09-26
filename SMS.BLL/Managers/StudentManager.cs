using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.BLL.Contracts.Base;
using SMS.DAL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class StudentManager :Manager<Student>, IStudentManager
    {
        private readonly IStudentRepository studentRepository;

        public StudentManager(IStudentRepository studentRepository):base(studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        public async Task<Student> GetStudentByClassRollAsync(int classRoll)
        {
            return await studentRepository.GetStudentByClassRollAsync(classRoll);
        }
    }
}

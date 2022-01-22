using SMS.BLL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IStudentManager : IManager<Student>
    {
        Task<Student> GetStudentByClassRollAsync(int classRoll);

        Task<Student> GetStudentByClassRollAsync(int id, int classRoll);

        Task<List<Student>> GetStudentsByClassIdAndSessionIdAsync(int sessionId, int classId);
        
    }
}

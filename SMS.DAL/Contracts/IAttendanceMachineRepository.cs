using SMS.DAL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts
{
    public interface IAttendanceMachineRepository : IRepository<Tran_MachineRawPunch>
    {
        Task<List<Tran_MachineRawPunch>> GetAllAttendanceByDateAsync(DateTime dateTime);
        Task<Tran_MachineRawPunch> GetTodaysAttendanceByUserIdAsync(int attendanceId);
    }
}

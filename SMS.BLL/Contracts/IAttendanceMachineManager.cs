using SMS.BLL.Contracts.Base;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IAttendanceMachineManager : IManager<Tran_MachineRawPunch>
    {
        Task<List<Tran_MachineRawPunch>> GetAllAttendanceByDateAsync(DateTime dateTime);
        Task<IEnumerable<AttendanceVM>> GetAttendanceByDateAsync(string attendanceFor, string date, string attendanceType, int? aSessionId, int? aClassId);
        
        Task<Tran_MachineRawPunch> GetTodaysAttendanceByUserIdAsync(int attendanceId);
    }
}

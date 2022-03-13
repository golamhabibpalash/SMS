using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class AttendanceMachineManager : Manager<Tran_MachineRawPunch>, IAttendanceMachineManager
    {
        private readonly IAttendanceMachineRepository _attendanceMachineRepository;

        public AttendanceMachineManager(IAttendanceMachineRepository attendanceMachineRepository) :base(attendanceMachineRepository)
        {
           _attendanceMachineRepository = attendanceMachineRepository;
        }

        public async Task<List<Tran_MachineRawPunch>> GetAllAttendanceByDateAsync(DateTime dateTime)
        {
            return await _attendanceMachineRepository.GetAllAttendanceByDateAsync(dateTime);
        }

        public async Task<Tran_MachineRawPunch> GetTodaysAttendanceByUserIdAsync(int attendanceId)
        {
            return await _attendanceMachineRepository.GetTodaysAttendanceByUserIdAsync(attendanceId);
        }
    }
}

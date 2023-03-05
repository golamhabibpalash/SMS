using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
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

        public async Task<IEnumerable<AttendanceVM>> GetAttendanceByDateAsync(string attendanceFor, string date, string attendanceType, int? aSessionId, int? aClassId)
        {
            var result =  await _attendanceMachineRepository.GetAttendanceByDateAsync(attendanceFor, date, attendanceType, aSessionId, aClassId);
            return result.OrderByDescending(m => m.CardNo);
        }

        public async Task<List<Tran_MachineRawPunch>> GetAttendanceByDateRangeAsync(string StartDate, string EndDate)
        {
            var result = await _attendanceMachineRepository.GetAttendanceByDateRangeAsync(StartDate, EndDate);
            return result;
        }

        public async Task<List<Tran_MachineRawPunch>> GetCheckinDataByDateAsync(string date)
        {
            List<Tran_MachineRawPunch> allCheckInAttendnace = new List<Tran_MachineRawPunch>();
            var result =await _attendanceMachineRepository.GetCheckinDataByDateAsync(date);
            if (result.Count>0)
            {
                allCheckInAttendnace = result;
            }
            return allCheckInAttendnace;
        }

        public async Task<List<Tran_MachineRawPunch>> GetCheckOutDataByDateAsync(string date)
        {
            List<Tran_MachineRawPunch> allCheckOutAttendnace = new List<Tran_MachineRawPunch>();
            var result = await _attendanceMachineRepository.GetCheckOutDataByDateAsync(date);
            if (result.Count > 0)
            {
                allCheckOutAttendnace = result;
            }
            return allCheckOutAttendnace;
        }

        public async Task<List<Student>> GetTodaysAbsentStudentAsync(string date)
        {
            List<Student> students = new List<Student>();
            students = await _attendanceMachineRepository.GetTodaysAbsentStudentAsync(date);
            return students;
        }

        public async Task<Tran_MachineRawPunch> GetTodaysAttendanceByUserIdAsync(int attendanceId)
        {
            return await _attendanceMachineRepository.GetTodaysAttendanceByUserIdAsync(attendanceId);
        }
    }
}

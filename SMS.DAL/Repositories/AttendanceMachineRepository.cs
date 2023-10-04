using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class AttendanceMachineRepository : Repository<Tran_MachineRawPunch>, IAttendanceMachineRepository
    {
        public AttendanceMachineRepository(ApplicationDbContext context) : base(context)
        {

        }


        public async Task<List<Tran_MachineRawPunch>> GetAllAttendanceByDateAsync(DateTime dateTime)
        {
            var pDateTime = new SqlParameter("date", dateTime.ToString("dd-MM-yyyy"));
            List<Tran_MachineRawPunch> allAttendance = await _context.Tran_MachineRawPunch.FromSqlInterpolated($"sp_Get_Checkin_Data {pDateTime}").ToListAsync();
            return allAttendance;

        }

        public async Task<IEnumerable<AttendanceVM>> GetAttendanceByDateAsync(string attendanceFor, string date, string attendanceType, int? aSessionId, int? aClassId)
        {
            //List<Tran_MachineRawPunch> tran_MachineRawPunches =await GetAllAttendanceByDateAsync(date.Date);
            var pAttendanceFor = new SqlParameter("attendanceFor", attendanceFor);
            var pDate = new SqlParameter("date", date);
            var pAttendanceType = new SqlParameter("attendanceType", attendanceType);
            var pASessionId = aSessionId!=null ? new SqlParameter("aSessionId", aSessionId) : null;
            var pClassId = aClassId!=null ? new SqlParameter("aClassId", aClassId) : null;
            var result = await _context.AttendanceVMs.FromSqlInterpolated($"sp_get_attendance_by_date {pAttendanceFor},{pDate},{pAttendanceType},{pASessionId},{pClassId}").ToArrayAsync();
            return result;            
        }

        public async Task<List<Tran_MachineRawPunch>> GetAttendanceByDateRangeAsync(string StartDate, string EndDate)
        {
            string sql = @"select t.* from Tran_MachineRawPunch t where Format(t.PunchDatetime,'yyyy-MM-dd') between convert(datetime,'"+StartDate+"') and convert(datetime,'"+EndDate+"')";
            var attendanceList = await _context.Tran_MachineRawPunch.FromSqlRaw(sql).ToListAsync();
            return attendanceList;
        }

        public async Task<List<Tran_MachineRawPunch>> GetAttendanceByMonthSingleStudent(int studentId, int monthId)
        {
            var stuId = new SqlParameter("studentId", studentId);
            var mId = new SqlParameter("monthId", monthId);
            List<Tran_MachineRawPunch> allAttendance = await _context.Tran_MachineRawPunch.FromSqlInterpolated($"sp_get_Attendance_by_Month_SingleStudent {stuId},{mId}").ToListAsync();
            return allAttendance;
        }

        public async Task<List<Tran_MachineRawPunch>> GetCheckinDataByDateAsync(string date)
        {
            var pDate = new SqlParameter("date", date);
            try
            {
                var result = await _context.Tran_MachineRawPunch.FromSqlInterpolated($"sp_Get_Checkin_Data {date}").ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Tran_MachineRawPunch>> GetCheckOutDataByDateAsync(string date)
        {
            var pDate = new SqlParameter("date", date);
            try
            {
                var result = await _context.Tran_MachineRawPunch.FromSqlInterpolated($"sp_Get_CheckOut_Data {date}").ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Employee>> GetTodaysAbsentEmployeeAsync(string date)
        {
            var pDate = new SqlParameter("date", date);
            try
            {
                var result = await _context.Employee.FromSqlInterpolated($"sp_get_todays_absent_employees_by_date {date}").ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Student>> GetTodaysAbsentStudentAsync(string date)
        {
            var pDate = new SqlParameter("date", date);
            try
            {
                var result = await _context.Student.FromSqlInterpolated($"sp_get_todays_absent_students_by_date {date}").ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Tran_MachineRawPunch> GetTodaysAttendanceByUserIdAsync(int attendanceId)
        {
            var allAttendance = await _context.Tran_MachineRawPunch.Where(t => t.PunchDatetime.Date == DateTime.Now.Date).ToListAsync();
            var existAttendance = await _context.Tran_MachineRawPunch.FirstOrDefaultAsync(m => m.CardNo == attendanceId.ToString() && m.PunchDatetime.Date == DateTime.Now.Date);

            return existAttendance;
        }


    }
}

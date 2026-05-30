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
            List<Tran_MachineRawPunch> allAttendance = await _context.Tran_MachineRawPunch
                .Where(t => t.PunchDatetime.Date == dateTime.Date)
                .ToListAsync();
            return allAttendance;
        }

        public async Task<IEnumerable<AttendanceVM>> GetAttendanceByDateAsync(string attendanceFor, string date, string attendanceType, int? aSessionId, int? aClassId)
        {
            DateTime parsedDate = DateTime.Parse(date);
            var rawPunches = await _context.Tran_MachineRawPunch
                .Where(t => t.PunchDatetime.Date == parsedDate.Date)
                .ToListAsync();

            var result = new List<AttendanceVM>();

            if (attendanceFor == "Student")
            {
                var studentsQuery = _context.Student
                    .Include(s => s.AcademicClass)
                    .Include(s => s.AcademicSection)
                    .AsQueryable();

                if (aClassId.HasValue)
                    studentsQuery = studentsQuery.Where(s => s.AcademicClassId == aClassId.Value);

                var students = await studentsQuery.ToListAsync();

                result = students.Select(s =>
                {
                    var punch = rawPunches.FirstOrDefault(r => r.CardNo == s.ClassRoll.ToString());
                    return new AttendanceVM
                    {
                        CardNo = s.ClassRoll.ToString(),
                        Name = s.Name,
                        Class_Designation = s.AcademicClass?.Name,
                        Phone = s.PhoneNo,
                        GuardianPhone = s.GuardianPhone,
                        PunchTime = punch?.PunchDatetime.ToString("hh:mm:ss tt"),
                        SectionId = s.AcademicSectionId
                    };
                }).ToList();
            }
            else if (attendanceFor == "Employee")
            {
                var employees = await _context.Employee
                    .Include(e => e.Designation)
                    .ToListAsync();

                result = employees.Select(e =>
                {
                    var punch = rawPunches.FirstOrDefault(r => r.CardNo == e.Id.ToString());
                    return new AttendanceVM
                    {
                        CardNo = e.Id.ToString(),
                        Name = e.EmployeeName,
                        Class_Designation = e.Designation?.DesignationName,
                        Phone = e.Phone,
                        GuardianPhone = "",
                        PunchTime = punch?.PunchDatetime.ToString("hh:mm:ss tt"),
                        SectionId = null
                    };
                }).ToList();
            }

            return result;
        }

        public async Task<List<Tran_MachineRawPunch>> GetAttendanceByDateRangeAsync(string StartDate, string EndDate)
        {
            DateTime start = DateTime.Parse(StartDate);
            DateTime end = DateTime.Parse(EndDate);

            var attendanceList = await _context.Tran_MachineRawPunch
                .Where(t => t.PunchDatetime.Date >= start.Date && t.PunchDatetime.Date <= end.Date)
                .ToListAsync();
            return attendanceList;
        }

        public async Task<List<Tran_MachineRawPunch>> GetAttendanceByMonthSingleStudent(int studentId, string monthYear)
        {
            if (monthYear.Length >= 6 && int.TryParse(monthYear[..4], out int year) && int.TryParse(monthYear[^2..], out int month))
            {
                List<Tran_MachineRawPunch> allAttendance = await _context.Tran_MachineRawPunch
                    .Where(t => t.PunchDatetime.Year == year && t.PunchDatetime.Month == month
                        && t.CardNo == studentId.ToString())
                    .ToListAsync();
                return allAttendance;
            }

            return new List<Tran_MachineRawPunch>();
        }

        public async Task<List<Tran_MachineRawPunch>> GetCheckinDataByDateAsync(string date)
        {
            DateTime parsedDate = DateTime.Parse(date);
            try
            {
                var result = await _context.Tran_MachineRawPunch
                    .Where(t => t.PunchDatetime.Date == parsedDate.Date)
                    .ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Tran_MachineRawPunch>> GetEmpCheckinDataByDateAsync(string date)
        {
            DateTime parsedDate = DateTime.Parse(date);
            try
            {
                var result = await _context.Tran_MachineRawPunch
                    .Where(t => t.PunchDatetime.Date == parsedDate.Date)
                    .ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Tran_MachineRawPunch>> GetCheckOutDataByDateAsync(string date)
        {
            DateTime parsedDate = DateTime.Parse(date);
            try
            {
                var result = await _context.Tran_MachineRawPunch
                    .Where(t => t.PunchDatetime.Date == parsedDate.Date)
                    .ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Employee>> GetTodaysAbsentEmployeeAsync(string date)
        {
            DateTime parsedDate = DateTime.Parse(date);
            try
            {
                var cardNosWithPunch = await _context.Tran_MachineRawPunch
                    .Where(t => t.PunchDatetime.Date == parsedDate.Date)
                    .Select(t => t.CardNo)
                    .Distinct()
                    .ToListAsync();

                var absentEmployees = await _context.Employee
                    .Where(e => !cardNosWithPunch.Contains(e.Id.ToString()))
                    .ToListAsync();

                return absentEmployees;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Student>> GetTodaysAbsentStudentAsync(string date)
        {
            DateTime parsedDate = DateTime.Parse(date);
            try
            {
                var cardNosWithPunch = await _context.Tran_MachineRawPunch
                    .Where(t => t.PunchDatetime.Date == parsedDate.Date)
                    .Select(t => t.CardNo)
                    .Distinct()
                    .ToListAsync();

                var absentStudents = await _context.Student
                    .Where(s => s.Status && !cardNosWithPunch.Contains(s.ClassRoll.ToString()))
                    .ToListAsync();

                return absentStudents;
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

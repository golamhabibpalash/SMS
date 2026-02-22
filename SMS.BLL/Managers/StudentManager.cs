using BLL.Managers.Base;
using Microsoft.EntityFrameworkCore;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using SMS.Entities.AdditionalModels.StudentVM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class StudentManager : Manager<Student>, IStudentManager
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IAttendanceMachineManager _attendanceMachineManager;
        private readonly IAcademicSessionManager _academicSessionManager;
        private readonly IOffDayManager _offDayManager;
        private readonly IInstituteManager _instituteManager;

        public StudentManager(IStudentRepository studentRepository, IAttendanceMachineManager attendanceMachineManager, IAcademicSessionManager academicSessionManager, IOffDayManager offDayManager, IInstituteManager instituteManager) : base(studentRepository)
        {
            _studentRepository = studentRepository;
            _attendanceMachineManager = attendanceMachineManager;
            _academicSessionManager = academicSessionManager;
            _offDayManager = offDayManager;
            _instituteManager = instituteManager;
        }

        public async Task<List<StudentListVM>> GetCurrentStudentListAsync(int? AcademicClassId, int? AcademicSectionId)
        {
            return await _studentRepository.GetCurrentStudentListAsync(AcademicClassId, AcademicSectionId);
        }

        public async Task<Student> GetStudentByClassRollAsync(int classRoll)
        {
            return await _studentRepository.GetStudentByClassRollAsync(classRoll);
        }
        public async Task<Student> GetStudentByUniqueIdAsync(string uniqueId)
        {
            return await _studentRepository.GetStudentByUniqueIdAsync(uniqueId);
        }

        public async Task<Student> GetStudentByClassRollAsync(int id, int classRoll)
        {
            return await _studentRepository.GetStudentByClassRollAsync(id, classRoll);
        }

        public async Task<List<Student>> GetStudentsByClassIdAndSessionIdAsync(int sessionId, int classId)
        {
            return await _studentRepository.GetStudentsByClassIdAndSessionIdAsync(sessionId, classId);
        }

        public async Task<List<Student>> GetStudentsByClassSessionSectionAsync(int sessionId, int classId, int sectionId)
        {
            return await _studentRepository.GetStudentsByClassSessionSectionAsync(sessionId, classId, sectionId);
        }
        public async Task<string> GetUniqueIdByStudentId(int stuId)
        {
            Student student = await _studentRepository.GetByIdAsync(stuId);
            return student.UniqueId;
        }

        public async Task<List<StudentListVM>> GetStudentsBySearch(string search)
        {
            search = search.Trim().ToLower();
            var students = await _studentRepository.GetAllAsync();

            students = students
                .Where(s =>
                    (s.Name?.ToLower().Contains(search) ?? false) ||
                    (s.NameBangla?.Contains(search) ?? false) ||
                    (s.UniqueId?.Contains(search) ?? false) ||
                    (s.AcademicClass?.Name?.ToLower().Contains(search) ?? false) ||
                    (s.ClassRoll.ToString().Contains(search)) ||
                    (s.AcademicSection?.Name?.ToLower().Contains(search) ?? false) ||
                    (s.PhoneNo?.Contains(search) ?? false) ||
                    (s.GuardianPhone?.Contains(search) ?? false))
                .ToList();


            List<StudentListVM> studentListVMs = new();

            studentListVMs = students.Select(student => new StudentListVM
            {
                Id = student.Id,
                ClassRoll = student.ClassRoll,
                Photo = student.Photo,
                StudentName = student.Name,
                NameBangla = student.NameBangla,
                ClassName = student.AcademicClass?.Name,
                SectionName = student.AcademicSection?.Name,
                PhoneNo = student.PhoneNo,
                SessionName = student.AcademicSession?.Name,
                Gender = student.Gender?.Name,
                Status = student.Status,
                ClassSerial = student.AcademicClass?.ClassSerial,
                IsResidential = student.IsResidential,
                UniqueId = student.UniqueId,
                GuardianPhone = student.GuardianPhone
            }).ToList();
            return studentListVMs;
        }

        public async Task<ProfileAttendance> GetProfileAttendanceAsync(int id)
        {
            Student student = await _studentRepository.GetByIdAsync(id);
            ProfileAttendance profileAttendance = new ProfileAttendance();
            var currentSession = await _academicSessionManager.GetCurrentAcademicSessionAsync();
            profileAttendance.CurrentSession = currentSession;
            Dictionary<int, int> monthWiseAttendance = new Dictionary<int, int>();
            var institute = await _instituteManager.GetByIdAsync(1);
            
            var currentSessonsAllAttendances = await _attendanceMachineManager.GetSessionWiseAttendanceByStudentUniqueIdAsync(currentSession.Id, student.UniqueId);
            if (currentSessonsAllAttendances!=null && currentSessonsAllAttendances?.Count>0)
            {
                currentSessonsAllAttendances = currentSessonsAllAttendances.DistinctBy(s => s.PunchDatetime.Date).ToList();

                
                foreach (var attendance in currentSessonsAllAttendances)
                {
                    //Late Arrivals
                    if (attendance.PunchDatetime.TimeOfDay > institute.StartingTime.TimeOfDay)
                    {
                        profileAttendance.LateArrivals += 1;
                    }
                    int currentMonth = attendance.PunchDatetime.Month;
                    if (monthWiseAttendance.ContainsKey(currentMonth))
                    {
                        monthWiseAttendance[currentMonth]++;
                    }
                    else
                    {
                        monthWiseAttendance[currentMonth] = 1;
                    }
                }
            }
            

            //Overall Attendance
            DateTime currentSessionStartDate = new DateTime(Convert.ToInt32(currentSession.Name.Substring(currentSession.Name.Length-4)), 1,1);
            int totalDaysInSession = (DateTime.Today - currentSessionStartDate).Days + 1;
            var sessionWiseTotalOffDays = await _offDayManager.GetYearlyHolidaysAsync(Convert.ToInt32(currentSession.Name.Substring(currentSession.Name.Length - 4)));
            int totalInstitutionOpendays = totalDaysInSession - sessionWiseTotalOffDays.Count;
            int totalPresent = Convert.ToInt32(currentSessonsAllAttendances?.Count());
            profileAttendance.OverallAttendance = (totalPresent * 100) / totalInstitutionOpendays;

            //Days Present
            profileAttendance.DaysPresent = Convert.ToInt32(currentSessonsAllAttendances?.Count());

            //Days Absent
            profileAttendance.DaysAbsent = totalInstitutionOpendays - totalPresent;

            //monthWiseAttendance
            foreach (var item in monthWiseAttendance)
            {
                string name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);
                var monthlyOffDays = sessionWiseTotalOffDays.Where(o => o.OffDayStartingDate.Day == item.Key).Count();
                var totalDaysInTheMonth = DateTime.DaysInMonth(Convert.ToInt32(currentSession.Name.Substring(currentSession.Name.Length - 4)), item.Key);
                int totalDays = 0;
                int percentage = 0;
                MonthlyAttendance monthlyAttendance = new()
                {
                    MonthName = name,
                    TotalDays =totalDays = totalDaysInTheMonth - monthlyOffDays,
                    DaysPresent = item.Value,
                    DaysAbsent = totalDays - item.Value,
                    LateArrivals = 0,
                    Percentage = percentage = (item.Value * 100) / (totalDaysInTheMonth - monthlyOffDays),
                    Status = GetAttendanceStatus(percentage)
                };
                profileAttendance.MonthlyAttendances.Add(monthlyAttendance);
            }

            return profileAttendance;
        }
        private string GetAttendanceStatus(int percentage)
        {
            if (percentage >= 80) return "Excellent";
            if (percentage >= 50) return "Good";
            if (percentage >= 26) return "Average";
            if (percentage >= 1) return "Poor";

            return "No Data";
        }

        public async Task<List<Student>> GetAllActiveStudentsAsync()
        {
            return await _studentRepository.Table.Where(s => s.Status == true).ToListAsync();
        }
    }
}

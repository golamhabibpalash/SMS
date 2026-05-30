using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts.Reports;
using SMS.DB;
using SMS.Entities.AdditionalModels;
using SMS.Entities.RptModels;
using SMS.Entities.RptModels.AttendanceVM;
using SMS.Entities.RptModels.Results;
using SMS.Entities.RptModels.StudentPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories.Reports
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RptAdmitCardVM>> GetAdmitCard(int monthId, int academicClassId, int academicSectionId, int examTypeId)
        {
            var query = _context.Student
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicSection)
                .Include(s => s.AcademicSession)
                .Include(s => s.Gender)
                .Include(s => s.Religion)
                .AsQueryable();

            var exams = await _context.AcademicExams
                .Include(e => e.AcademicExamGroup)
                .Include(e => e.AcademicSubject)
                .Where(e => e.AcademicExamGroup.ExamMonthId == monthId
                    && e.AcademicExamGroup.AcademicExamTypeId == examTypeId)
                .ToListAsync();

            if (academicClassId > 0)
            {
                var examClassIds = exams.Where(e => e.AcademicClassId == academicClassId).Select(e => e.AcademicClassId).Distinct();
                query = query.Where(s => s.AcademicClassId == academicClassId);
                exams = exams.Where(e => e.AcademicClassId == academicClassId).ToList();
            }

            if (academicSectionId > 0)
            {
                query = query.Where(s => s.AcademicSectionId == academicSectionId);
            }

            var institute = await _context.Institute.FirstOrDefaultAsync();

            var students = await query.ToListAsync();

            var admitCards = students
                .SelectMany(s => exams.Where(e =>
                    e.AcademicClassId == s.AcademicClassId
                    && (!e.AcademicSectionId.HasValue || e.AcademicSectionId == s.AcademicSectionId))
                    .Select(e => new RptAdmitCardVM
                    {
                        StudentId = s.Id,
                        ClassRoll = s.ClassRoll,
                        StudentName = s.Name,
                        FatherName = s.FatherName,
                        MotherName = s.MotherName,
                        SessionName = s.AcademicSession?.Name,
                        ClassName = s.AcademicClass?.Name,
                        SectionName = s.AcademicSection?.Name,
                        AcademicSectionId = s.AcademicSectionId,
                        MonthId = monthId,
                        SubjectCode = e.AcademicSubject?.Id,
                        SubjectName = e.AcademicSubject?.SubjectName,
                        AcademicClassId = s.AcademicClassId,
                        ExamTypeName = e.AcademicExamGroup?.AcademicExamType?.ExamTypeName,
                        InstituteName = institute?.Name,
                        EIIN = institute?.EIIN,
                        Gender = s.Gender?.Name,
                        Religion = s.Religion?.Name,
                        StudentStauts = s.Status
                    }))
                .OrderBy(a => a.ClassRoll)
                .ToList();

            return admitCards;
        }

        public async Task<List<rptStudentPaymentsVM>> GetStudentPaymentsByRoll(int classRoll, string fromDate, string toDate)
        {
            List<rptStudentPaymentsVM> rptStudentPaymentsVMs = new List<rptStudentPaymentsVM>();

            var student = await _context.Student.FirstOrDefaultAsync(s => s.ClassRoll == classRoll);
            if (student == null) return rptStudentPaymentsVMs;

            DateTime from = DateTime.Parse(fromDate);
            DateTime to = DateTime.Parse(toDate);

            rptStudentPaymentsVMs = await _context.StudentPayment
                .Where(sp => sp.StudentId == student.Id && sp.PaidDate >= from && sp.PaidDate <= to)
                .Select(sp => new rptStudentPaymentsVM
                {
                    ReceiptNo = sp.ReceiptNo,
                    PaidDate = sp.PaidDate.ToString("yyyy-MM-dd"),
                    PaymentTypeName = "",
                    TotalPayment = sp.TotalPayment,
                    Remarks = sp.Remarks
                })
                .ToListAsync();

            return rptStudentPaymentsVMs;
        }

        public async Task<List<RptStudentVM>> getStudentsInfo(int AcademicSessionId, int? AcademicClassId, int? AcademicSectionId)
        {
            var query = _context.Student
                .Include(s => s.AcademicClass)
                .Include(s => s.AcademicSection)
                .Include(s => s.AcademicSession)
                .Include(s => s.Gender)
                .Include(s => s.Religion)
                .Include(s => s.BloodGroup)
                .Include(s => s.Nationality)
                .Where(s => s.AcademicSessionId == AcademicSessionId)
                .AsQueryable();

            if (AcademicClassId.HasValue)
                query = query.Where(s => s.AcademicClassId == AcademicClassId.Value);

            if (AcademicSectionId.HasValue)
                query = query.Where(s => s.AcademicSectionId == AcademicSectionId.Value);

            var rptStudentVMs = await query
                .Select(s => new RptStudentVM
                {
                    ClassRoll = s.ClassRoll.ToString(),
                    StudentName = s.Name,
                    ClassName = s.AcademicClass.Name,
                    SessionName = s.AcademicSession.Name,
                    FatherName = s.FatherName,
                    MotherName = s.MotherName,
                    GuardianPhone = s.GuardianPhone,
                    PhoneNo = s.PhoneNo,
                    Gender = s.Gender.Name,
                    Religion = s.Religion.Name,
                    Status = s.Status ? "Active" : "Inactive",
                    BloodGroup = s.BloodGroup.Name,
                    SectionName = s.AcademicSection.Name,
                    AcademicClassId = s.AcademicClassId.ToString(),
                    AcademicSectionId = s.AcademicSectionId.ToString(),
                    UniqueId = s.UniqueId
                })
                .ToListAsync();

            return rptStudentVMs;
        }
        public async Task<List<RptStudentsPaymentVM>> GetStudentPayment(string fromDate, string ToDate, string AcademicClassId, string AcademicSectionId)
        {
            DateTime from = DateTime.Parse(fromDate);
            DateTime to = DateTime.Parse(ToDate);

            var query = _context.StudentPayment
                .Include(sp => sp.Student).ThenInclude(s => s.AcademicClass)
                .Include(sp => sp.Student).ThenInclude(s => s.AcademicSection)
                .Include(sp => sp.StudentPaymentDetails)
                .Where(sp => sp.PaidDate >= from && sp.PaidDate <= to)
                .AsQueryable();

            if (!string.IsNullOrEmpty(AcademicClassId) && AcademicClassId != "null" && int.TryParse(AcademicClassId, out int classId))
            {
                query = query.Where(sp => sp.Student.AcademicClassId == classId);
            }

            if (!string.IsNullOrEmpty(AcademicSectionId) && AcademicSectionId != "null" && int.TryParse(AcademicSectionId, out int sectionId))
            {
                query = query.Where(sp => sp.Student.AcademicSectionId == sectionId);
            }

            var rptStudentsPayments = await query
                .Select(sp => new RptStudentsPaymentVM
                {
                    ClassRoll = sp.Student.ClassRoll.ToString(),
                    StudentName = sp.Student.Name,
                    AcademicSection = sp.Student.AcademicSection.Name,
                    PaymentType = "",
                    ReceiptNo = sp.ReceiptNo,
                    Remarks = sp.Remarks,
                    PaidDate = sp.PaidDate.ToString("yyyy-MM-dd"),
                    TotalPayment = sp.TotalPayment,
                    AcademicClassId = sp.Student.AcademicClassId.ToString(),
                    AcademicSectionId = sp.Student.AcademicSectionId.ToString(),
                    AcademicClassName = sp.Student.AcademicClass.Name,
                    IsResidential = sp.Student.IsResidential
                })
                .ToListAsync();

            return rptStudentsPayments;
        }

        public async Task<List<RptDailyAttendaceVM>> GetDailyAttendanceReport(string fromDate, string AcademicClassId, string AcademicSectionId, string attendanceType, string aSessionId, string attendanceFor)
        {
            DateTime date = DateTime.Parse(fromDate);

            var rawPunches = await _context.Tran_MachineRawPunch
                .Where(t => t.PunchDatetime.Date == date.Date)
                .ToListAsync();

            var result = new List<RptDailyAttendaceVM>();

            if (attendanceFor == "Student")
            {
                var studentsQuery = _context.Student
                    .Include(s => s.AcademicClass)
                    .Include(s => s.AcademicSection)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(AcademicClassId) && int.TryParse(AcademicClassId, out int cId))
                    studentsQuery = studentsQuery.Where(s => s.AcademicClassId == cId);

                if (!string.IsNullOrEmpty(AcademicSectionId) && int.TryParse(AcademicSectionId, out int sId))
                    studentsQuery = studentsQuery.Where(s => s.AcademicSectionId == sId);

                var students = await studentsQuery.ToListAsync();

                result = students.Select(s =>
                {
                    var punch = rawPunches.FirstOrDefault(r => r.CardNo == s.ClassRoll.ToString());
                    return new RptDailyAttendaceVM
                    {
                        CardNo = s.ClassRoll.ToString(),
                        ClassRoll = s.ClassRoll.ToString(),
                        Name = s.Name,
                        Class_Designation = s.AcademicClass?.Name,
                        Phone = s.PhoneNo,
                        GuardianPhone = s.GuardianPhone,
                        PunchTime = punch?.PunchDatetime.ToString("hh:mm:ss tt"),
                        SortingOrder = s.AcademicClass?.ClassSerial.ToString(),
                        SMSSent = "",
                        ClassSL = s.AcademicClass?.ClassSerial.ToString()
                    };
                }).OrderBy(r => r.SortingOrder).ThenBy(r => r.ClassRoll).ToList();
            }
            else if (attendanceFor == "Employee")
            {
                var employees = await _context.Employee
                    .Include(e => e.Designation)
                    .ToListAsync();

                result = employees.Select(e =>
                {
                    var punch = rawPunches.FirstOrDefault(r => r.CardNo == e.Id.ToString());
                    return new RptDailyAttendaceVM
                    {
                        CardNo = e.Id.ToString(),
                        ClassRoll = "",
                        Name = e.EmployeeName,
                        Class_Designation = e.Designation?.DesignationName,
                        Phone = e.Phone,
                        GuardianPhone = "",
                        PunchTime = punch?.PunchDatetime.ToString("hh:mm:ss tt"),
                        SortingOrder = "",
                        SMSSent = "",
                        ClassSL = ""
                    };
                }).ToList();
            }

            return result;
        }

        public async Task<List<RptDailyAttendaceVM>> GetDailyAttendanceReportCheckOut(string fromDate, string AcademicClassId, string AcademicSectionId, string attendanceFor)
        {
            DateTime date = DateTime.Parse(fromDate);

            var checkOutPunches = await _context.Tran_MachineRawPunch
                .Where(t => t.PunchDatetime.Date == date.Date)
                .ToListAsync();

            var result = new List<RptDailyAttendaceVM>();

            if (attendanceFor == "Student")
            {
                var studentsQuery = _context.Student
                    .Include(s => s.AcademicClass)
                    .Include(s => s.AcademicSection)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(AcademicClassId) && int.TryParse(AcademicClassId, out int cId))
                    studentsQuery = studentsQuery.Where(s => s.AcademicClassId == cId);

                if (!string.IsNullOrEmpty(AcademicSectionId) && int.TryParse(AcademicSectionId, out int sId))
                    studentsQuery = studentsQuery.Where(s => s.AcademicSectionId == sId);

                var students = await studentsQuery.ToListAsync();

                result = students.Select(s =>
                {
                    var punch = checkOutPunches.FirstOrDefault(r => r.CardNo == s.ClassRoll.ToString());
                    return new RptDailyAttendaceVM
                    {
                        CardNo = s.ClassRoll.ToString(),
                        ClassRoll = s.ClassRoll.ToString(),
                        Name = s.Name,
                        Class_Designation = s.AcademicClass?.Name,
                        Phone = s.PhoneNo,
                        GuardianPhone = s.GuardianPhone,
                        PunchTime = punch?.PunchDatetime.ToString("hh:mm:ss tt"),
                        SortingOrder = s.AcademicClass?.ClassSerial.ToString(),
                        SMSSent = "",
                        ClassSL = s.AcademicClass?.ClassSerial.ToString()
                    };
                }).OrderBy(r => r.SortingOrder).ThenBy(r => r.ClassRoll).ToList();
            }
            else if (attendanceFor == "Employee")
            {
                var employees = await _context.Employee
                    .Include(e => e.Designation)
                    .ToListAsync();

                result = employees.Select(e =>
                {
                    var punch = checkOutPunches.FirstOrDefault(r => r.CardNo == e.Id.ToString());
                    return new RptDailyAttendaceVM
                    {
                        CardNo = e.Id.ToString(),
                        ClassRoll = "",
                        Name = e.EmployeeName,
                        Class_Designation = e.Designation?.DesignationName,
                        Phone = e.Phone,
                        GuardianPhone = "",
                        PunchTime = punch?.PunchDatetime.ToString("hh:mm:ss tt"),
                        SortingOrder = "",
                        SMSSent = "",
                        ClassSL = ""
                    };
                }).ToList();
            }

            return result;
        }
        public async Task<List<RptPaymentReceiptVM>> GetPaymentReceiptReport(int paymentId)
        {
            List<RptPaymentReceiptVM> rptPaymentReceiptVMs;
            try
            {
                rptPaymentReceiptVMs = await _context.StudentPayment
                    .Include(sp => sp.Student).ThenInclude(s => s.AcademicClass)
                    .Include(sp => sp.Student).ThenInclude(s => s.AcademicSection)
                    .Include(sp => sp.Student).ThenInclude(s => s.AcademicSession)
                    .Include(sp => sp.StudentPaymentDetails).ThenInclude(pd => pd.StudentFeeHead)
                    .Where(sp => sp.Id == paymentId)
                    .SelectMany(sp => sp.StudentPaymentDetails.Select(pd => new RptPaymentReceiptVM
                    {
                        ReceiptNo = sp.ReceiptNo,
                        Student_Name = sp.Student.Name,
                        Class_Name = sp.Student.AcademicClass.Name,
                        PaidDate = sp.PaidDate,
                        ClassRoll = sp.Student.ClassRoll,
                        Section_Name = sp.Student.AcademicSection.Name,
                        Fee_Head = pd.StudentFeeHead.Name,
                        PaidAmount = pd.PaidAmount,
                        TotalPayment = sp.TotalPayment
                    }))
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return rptPaymentReceiptVMs;
        }
        public async Task<List<SubjectWiseMarkSheetVM>> GetSubjectWiseMarkSheet(int examId)
        {
            var result = await _context.AcademicExamDetails
                .Include(aed => aed.AcademicExam).ThenInclude(e => e.AcademicSubject)
                .Include(aed => aed.Student)
                .Where(aed => aed.AcademicExamId == examId)
                .Select(aed => new SubjectWiseMarkSheetVM
                {
                    ObtainMark = aed.ObtainMark,
                    Name = aed.Student.Name,
                    StudentId = aed.StudentId,
                    ClassRoll = aed.Student.ClassRoll,
                    Status = aed.Status,
                    Remarks = aed.Remarks,
                    AcademicExamId = aed.AcademicExamId,
                    LetterGrade = "",
                    GradePoint = 0
                })
                .OrderBy(r => r.ClassRoll)
                .ToListAsync();

            return result;
        }
        public async Task<List<StudentWiseMarkSheetVM>> GetStudentWiseMarkSheet(int examGroupId, int classId)
        {
            var result = await _context.AcademicExams
                .Include(e => e.AcademicExamGroup)
                .Include(e => e.AcademicClass)
                .Include(e => e.AcademicSection)
                .Include(e => e.AcademicSubject)
                .Include(e => e.AcademicExamDetails).ThenInclude(aed => aed.Student).ThenInclude(s => s.Gender)
                .Include(e => e.AcademicExamDetails).ThenInclude(aed => aed.Student).ThenInclude(s => s.Religion)
                .Where(e => e.AcademicExamGroupId == examGroupId && e.AcademicClassId == classId)
                .SelectMany(e => e.AcademicExamDetails.Select(aed => new StudentWiseMarkSheetVM
                {
                    ExamGroupName = e.AcademicExamGroup.ExamGroupName,
                    ClassName = e.AcademicClass.Name,
                    StudentName = aed.Student.Name,
                    FatherName = aed.Student.FatherName,
                    MotherName = aed.Student.MotherName,
                    ClassRoll = aed.Student.ClassRoll,
                    SectionName = aed.Student.AcademicSection.Name,
                    AcademicSectionId = aed.Student.AcademicSectionId,
                    GenderName = aed.Student.Gender.Name,
                    SubjectName = e.AcademicSubject.SubjectName,
                    TotalMark = e.TotalMarks,
                    ObtainMark = aed.ObtainMark,
                    GPA = 0,
                    Grade = "",
                    MaxNumber = e.TotalMarks,
                    FinalGPA = 0,
                    FinalGrade = "",
                    AttendancePercentage = 0,
                    TotalObtainMarks = 0,
                    TotalFails = 0,
                    MeritPosition = 0,
                    GradeComments = "",
                    ExamGroupId = examGroupId,
                    AcademicClassId = classId,
                    StudentId = aed.StudentId,
                    DOB = aed.Student.DOB,
                    ReligionName = aed.Student.Religion.Name,
                    CreatedAt = aed.CreatedAt
                }))
                .OrderBy(r => r.ClassRoll)
                .ToListAsync();

            return result;
        }
    }
}

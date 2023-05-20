using SMS.BLL.Contracts.Reports;
using SMS.DAL.Contracts.Reports;
using SMS.Entities;
using SMS.Entities.RptModels;
using SMS.Entities.RptModels.AttendanceVM;
using SMS.Entities.RptModels.StudentPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers.Reports
{
    public class ReportManager : IReportManager
    {
        private readonly IReportRepository _reportRepository;
        public ReportManager(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<List<RptAdmitCardVM>> GetAdmitCard(int monthId, int academicClassId, int academicSectionId)
        {
            var admitCards = await _reportRepository.GetAdmitCard(monthId, academicClassId, academicSectionId);
            return admitCards.OrderBy(s => s.ClassRoll).ToList();
        }

        public async Task<List<RptDailyAttendaceVM>> GetDailyAttendanceReport(string fromDate, string academicClassId, string academicSectionId,string attendanceType)
        {
            return await _reportRepository.GetDailyAttendanceReport(fromDate, academicClassId, academicSectionId, attendanceType);
        }

        public async Task<List<RptStudentsPaymentVM>> GetStudentPayment(string fromDate, string ToDate, string AcademicClassId, string AcademicSectionId)
        {
            return await _reportRepository.GetStudentPayment(fromDate,ToDate,AcademicClassId, AcademicSectionId);
        }

        public async Task<List<rptStudentPaymentsVM>> GetStudentPaymentsByRoll(int classRoll, string fromDate, string toDate)
        {
           return await _reportRepository.GetStudentPaymentsByRoll(classRoll,fromDate,toDate);
        }

        public async Task<List<RptStudentVM>> GetStudentsInfo(int AcademicSessionId, int? AcademicClassId, int? AcademicSectionId)
        {
            return await _reportRepository.getStudentsInfo(AcademicSessionId, AcademicClassId, AcademicSectionId);
        }
    }
}

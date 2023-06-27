﻿using SMS.Entities.RptModels;
using SMS.Entities.RptModels.AttendanceVM;
using SMS.Entities.RptModels.StudentPayment;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts.Reports
{
    public interface IReportManager
    {
        Task<List<RptStudentVM>> GetStudentsInfo(int AcademicSessionId, int? AcademicClassId, int? AcademicSectionId);
        Task<List<rptStudentPaymentsVM>> GetStudentPaymentsByRoll(int classRoll, string fromDate, string toDate); 
        Task<List<RptAdmitCardVM>> GetAdmitCard(int monthId, int academicClassId, int academicSectionId);
        Task<List<RptStudentsPaymentVM>> GetStudentPayment(string fromDate, string ToDate, string AcademicClassId, string AcademicSectionId);
        Task<List<RptDailyAttendaceVM>> GetDailyAttendanceReport(string fromDate, string AcademicClassId, string AcademicSectionId,string attendanceType, string aSessionId, string attendanceFor);
        Task<List<RptPaymentReceiptVM>> GetPaymentReceiptReport(int paymentId);
    }
}

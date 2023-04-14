﻿using SMS.BLL.Contracts.Reports;
using SMS.DAL.Contracts.Reports;
using SMS.Entities.RptModels;
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

        public async Task<List<rptStudentPaymentsVM>> GetStudentPaymentsByRoll(int classRoll)
        {
           return await _reportRepository.GetStudentPaymentsByRoll(classRoll);
        }

        public async Task<List<RptStudentVM>> GetStudentsInfo()
        {
            return await _reportRepository.getStudentsInfo();
        }
    }
}

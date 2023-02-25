using SMS.Entities.RptModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Contracts.Reports
{
    public interface IReportRepository
    {
        Task<List<RptStudentVM>> getStudentsInfo();
    }
}

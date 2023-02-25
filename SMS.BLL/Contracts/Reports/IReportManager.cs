using SMS.BLL.Contracts.Base;
using SMS.Entities.RptModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts.Reports
{
    public interface IReportManager
    {
        Task<List<RptStudentVM>> getStudentsInfo();
    }
}

using SMS.Entities.RptModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts.Reports
{
    public interface IReportManager
    {
        Task<List<RptStudentVM>> GetStudentsInfo();
        Task<List<rptStudentPaymentsVM>> GetStudentPaymentsByRoll(int classRoll); 
        Task<List<RptAdmitCardVM>> GetAdmitCard(int monthId, int academicClassId, int academicSectionId);
    }
}

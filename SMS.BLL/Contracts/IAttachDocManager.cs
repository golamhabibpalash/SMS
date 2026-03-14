using System.Collections.Generic;
using System.Threading.Tasks;
using SMS.BLL.Contracts.Base;
using SMS.Entities;
using SMS.Entities.AdditionalModels.StudentVM;

namespace SMS.BLL.Contracts;

public interface IAttachDocManager : IManager<AttachDoc>
{
    Task<ProfileDocument> GetAllDocumentsByStudentId(int studentId);
}

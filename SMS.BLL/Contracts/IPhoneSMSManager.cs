using SMS.BLL.Contracts.Base;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Contracts
{
    public interface IPhoneSMSManager : IManager<PhoneSMS>
    {
        Task<bool> IsSMSSendForAttendance(string phoneNumber,string smsType,string dateTime);
    }
}

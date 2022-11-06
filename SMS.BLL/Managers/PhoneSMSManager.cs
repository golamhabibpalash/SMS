using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class PhoneSMSManager : Manager<PhoneSMS>, IPhoneSMSManager
    {
        private readonly IPhoneSMSRepository _phoneSMSRepository;
        public PhoneSMSManager(IPhoneSMSRepository phoneSMSRepository) : base(phoneSMSRepository)
        {
            _phoneSMSRepository = phoneSMSRepository;
        }

        public async Task<bool> IsSMSSendForAttendance(string phoneNumber, string smsType, string dateTime)
        {
            try
            {
                bool isSend = await _phoneSMSRepository.IsSMSSendForAttendance(phoneNumber, smsType, dateTime);
                return isSend;
            }
            catch (Exception)
            {
                throw;
            }
            

        }
    }
}

using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class PhoneSMSRepository : Repository<PhoneSMS>, IPhoneSMSRepository
    {
        public PhoneSMSRepository(ApplicationDbContext context):base(context)
        {
        }

        public async Task<bool> IsSMSSendForAttendance(string phoneNumber, string smsType, string dateTime)
        {
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(smsType) || string.IsNullOrEmpty(dateTime))
            {
                Exception ex = new Exception();
                throw ex;
            }
            try
            {
                DateTime parsedDate = DateTime.Parse(dateTime);
                var smsExists = await _context.PhoneSMS
                    .AnyAsync(s => s.MobileNumber == phoneNumber && s.SMSType == smsType && s.CreatedAt.Date == parsedDate.Date);
                return smsExists;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

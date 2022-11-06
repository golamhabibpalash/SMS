using Microsoft.EntityFrameworkCore;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class PhoneSMSRepository : Repository<PhoneSMS>, IPhoneSMSRepository
    {
        //private readonly ApplicationDbContext _context;
        public PhoneSMSRepository(ApplicationDbContext context):base(context)
        {
            //_context = context;
        }

        public async Task<bool> IsSMSSendForAttendance(string phoneNumber, string smsType, string dateTime)
        {
            //PhoneSMS sms = null;
            
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(smsType) || string.IsNullOrEmpty(dateTime))
            {
                Exception ex = new Exception();
                throw ex;
            }
            try
            {
                var sms = await _context.PhoneSMS.FromSqlInterpolated($"sp_Get_SMS_By_phone_type_date {phoneNumber}, {smsType}, {dateTime}").ToListAsync();
                if (sms.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
            

        }
    }
}

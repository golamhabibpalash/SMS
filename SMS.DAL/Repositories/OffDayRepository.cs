using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.DAL.Repositories
{
    public class OffDayRepository:Repository<OffDay>,IOffDayRepository
    {
        private new readonly ApplicationDbContext _context;
        public OffDayRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<List<DateTime>> GetMonthlyHolidaysAsync(string monthYear)
        {
            int a = Convert.ToInt32(monthYear.Substring(monthYear.Length - 4, 4));
            int b = Convert.ToInt32(monthYear[..2]);
            DateTime dateTime = new(a, b,1);
            List<DateTime> monthlyHolidays = new();
            List<OffDay> holidays = new();
            holidays= await _context.OffDays.Where(m =>m.OffDayStartingDate.Month==dateTime.Month).ToListAsync();
            if (holidays!=null)
            {
                foreach (var holiday in holidays)
                {
                    DateTime dt = holiday.OffDayStartingDate;
                    DateTime nextDate = dt;
                    for (int i = 0; i <= holiday.TotalDays; i++)
                    {
                        nextDate.AddDays(1);
                        if (nextDate.ToString("MM")!=dt.ToString("MM"))
                        {
                            break;
                        }
                        nextDate.ToString("MM");
                        bool isExist = (from m in monthlyHolidays
                                       where m.Date.ToString("ddMMyyyy") == nextDate.ToString("ddMMyyyy")
                                       select m).Any();
                        if (isExist==false)
                        {
                            monthlyHolidays.Add(nextDate);
                        }
                    }
                }
            }
            return monthlyHolidays;
        }
    }
}

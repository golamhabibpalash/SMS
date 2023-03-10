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
    public class OffDayManager:Manager<OffDay>,IOffDayManager
    {
        private readonly IOffDayRepository _offDayRepository;
        public OffDayManager(IOffDayRepository offDayRepository):base(offDayRepository)
        {
            _offDayRepository = offDayRepository;
        }

        public async Task<List<DateTime>> GetMonthlyHolidaysAsync(string monthYear)
        {
            List<DateTime> dateTimes = new List<DateTime>();
            dateTimes = await _offDayRepository.GetMonthlyHolidaysAsync(monthYear);
            return dateTimes;
        }

    }
}

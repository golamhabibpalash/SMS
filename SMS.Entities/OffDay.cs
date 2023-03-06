using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class OffDay:CommonProps
    {
        public DateTime OffDayDate { get; set; }
        public string Description { get; set; }
        public int OffDayTypeId { get; set; }
        public OffDayType OffDayType { get; set; }
    }
}

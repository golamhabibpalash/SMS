using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class EmployeeActivateHist : CommonProps
    {
        public int EmployeeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime ActionDateTime { get; set; } = DateTime.Now;
        [StringLength(5)]
        public string LastAction { get; set; }
    }
}

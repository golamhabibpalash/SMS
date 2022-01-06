using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Tran_MachineRawPunch
    {
        [Key]
        public int Tran_MachineRawPunchId { get; set; }
        [StringLength(10)]
        public string CardNo { get; set; }
        public DateTime PunchDatetime { get; set; }
        public char P_Day { get; set; }
        public char ISManual { get; set; }

        [StringLength(50)]
        public string PayCode { get; set; }

        [StringLength(5)]
        public string MachineNo { get; set; }
    }
}

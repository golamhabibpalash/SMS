using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class PhoneSMS : CommonProps
    {
        public string Text { get; set; }
        public string MobileNumber { get; set; }
        [StringLength(15)]
        public string SMSType { get; set; } //CheckIn, CheckOut, Notification, Payment, General

    }
}

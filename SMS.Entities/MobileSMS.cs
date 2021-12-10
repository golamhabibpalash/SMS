using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class PhoneSMS : CommonProps
    {
        public string Text { get; set; }
        public string MobileNumber { get; set; }
    }
}

using SMS.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{   
    public class Notification:CommonProps
    {
        public int NotificationEventId { get; set; }
        public NotificationEvent NotificationEvent { get; set; }
        public NotificationOption NotificationOption { get; set; }
        public string NotificationText { get; set; }
    }
}

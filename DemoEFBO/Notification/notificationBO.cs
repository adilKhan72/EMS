using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Notification
{
    public class notificationBO
    {
        public string UserId { get; set; }
    }
    public class notificationReadBO
    {
        public int UserId { get; set; }
       public int NotificationId { get; set; }
    }
}

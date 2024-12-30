using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Notification
{
    public class NotificationResponseBO
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string NotificationMsg { get; set; }
        public string CreationDateTime { get; set; }
        public int isRead { get; set; }
        public string NotificationHeader { get; set; }
        public int isSeen { get; set; }


    }
}

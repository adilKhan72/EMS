using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Notification
{
    public class NotificationFinalResponse
    {
        public bool IsSuccess { get; set; }
        public List<NotificationResponseBO> Lstdata { get; set; }
        public string ResponseMessage { get; set; }
    }
}

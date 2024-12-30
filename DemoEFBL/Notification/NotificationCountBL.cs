using DemoEFBO.Notification;
using DemoEFDAL.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.Notification
{
   public class NotificationCountBL
    {
        public int GetnotificationcountBL(int id)
        {
            try
            {
                notificationCountDAL objDAL = new notificationCountDAL();
                var objResult = objDAL.NotificationCount(id);
                if (objResult != null && objResult.count >= 0)
                {
                    return Convert.ToInt32(objResult.count);
                }
                return 0;
            }

            catch (Exception)
            {
                return 0;

            }



        }
    }
}

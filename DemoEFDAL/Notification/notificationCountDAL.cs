using DemoEFBO.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Notification
{
   public class notificationCountDAL
    {
        public sp_userNotificationCount_Result NotificationCount(int userID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.sp_userNotificationCount(userID).FirstOrDefault();// .sp_user sp_userNotificationCount(userID);

                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

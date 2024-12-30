using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Notification
{
    public class notificationDAL
    {
        public List<sp_userNotification_Result> GetNotificationByID(int UserId)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_userNotification(UserId).ToList();
                    if(result!=null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public void SetNotificationReadByID(int UserId, int NotificationId)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_userNotificationRead1(UserId, NotificationId);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void SetNotificationSeenByID(int UserId)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_userNotificationSeen(UserId);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public List<sp_unReaduserNotification_Result> GetUnReadNotificationByID(int UserId)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_unReaduserNotification(UserId).ToList();
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

        public sp_userNotificationDelete_Result NotificationDeleteDAL(string notificationID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.sp_userNotificationDelete(notificationID).SingleOrDefault();

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.Notification;
using DemoEFDAL.Notification;

namespace DemoEFBL.Notification
{
    public class notificationBL
    {
        public NotificationDeleteResponceBO NotificationDelete(string notificationID)
        {
            try
            {
                if (notificationID.Length > 0)
                {
                    NotificationDeleteResponceBO objBusinessObject = new NotificationDeleteResponceBO();
                    notificationDAL objDal = new notificationDAL();
                    var objResult = objDal.NotificationDeleteDAL(notificationID);
                    if (objResult != null)
                    {
                        objBusinessObject.ResponseCode = objResult.responseCode;
                        objBusinessObject.ResponseMessage = objResult.responsemsg;

                        return objBusinessObject;
                    }
                    return null;
                }
                return null;
            }
            catch
            {
                return null;

            }
        }

        public List<NotificationResponseBO> getnotificationmessage(int UserID)
        {

            try
            {
                if (UserID>0)
                {
                    List<NotificationResponseBO> Notification = new List<NotificationResponseBO>();
                    notificationDAL objDAL = new notificationDAL();
                    var objResult = objDAL.GetNotificationByID(UserID);
                    if(objResult!=null)
                    {
                        for (int i = 0; i < objResult.Count; i++)
                        {
                            Notification.Add(new NotificationResponseBO
                            {
                                NotificationId = objResult[i].NotificationID,
                                UserId = Convert.ToInt32(objResult[i].UserID),
                                NotificationMsg = objResult[i].NotificationMessage,
                                CreationDateTime = Convert.ToString(objResult[i].CreationDateTime),
                                isRead = Convert.ToInt32(objResult[i].isRead),
                                NotificationHeader = objResult[i].NotificationHeader,
                                isSeen = Convert.ToInt32(objResult[i].isSeen)
                            });
                        }
                        return Notification;
                    }
                }
                return null;
            }
            catch { return null; }
        }

        public bool setReadNotificationBL(notificationReadBO obj)
        {

            try
            {
                if (obj.UserId > 0)
                {
                    notificationDAL objDAL = new notificationDAL();
                    objDAL.SetNotificationReadByID(obj.UserId,obj.NotificationId);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        public bool setSeenNotificationBL(notificationReadBO obj)
        {

            try
            {
                if (obj.UserId > 0)
                {
                    notificationDAL objDAL = new notificationDAL();
                    objDAL.SetNotificationSeenByID(obj.UserId);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }
        public List<NotificationResponseBO> getunReadnotificationmessage(int UserID)
        {

            try
            {
                if (UserID > 0)
                {
                    List<NotificationResponseBO> Notification = new List<NotificationResponseBO>();
                    notificationDAL objDAL = new notificationDAL();
                    var objResult = objDAL.GetUnReadNotificationByID(UserID);
                    if (objResult != null)
                    {
                        for (int i = 0; i < objResult.Count; i++)
                        {
                            Notification.Add(new NotificationResponseBO
                            {
                                NotificationId = objResult[i].NotificationID,
                                UserId = Convert.ToInt32(objResult[i].UserID),
                                NotificationMsg = objResult[i].NotificationMessage,
                                CreationDateTime = Convert.ToString(objResult[i].CreationDateTime),
                                isRead = Convert.ToInt32(objResult[i].isRead),
                                NotificationHeader = objResult[i].NotificationHeader,
                                isSeen = Convert.ToInt32(objResult[i].isSeen)
                            }) ;
                        }
                        return Notification;
                    }
                }
                return null;
            }
            catch { return null; }
        }



    }
}

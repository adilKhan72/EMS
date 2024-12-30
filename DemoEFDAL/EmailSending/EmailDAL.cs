using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.EmailSending
{
    public class EmailDAL : IEmail
    {
        public List<sp_GetDashboardStats1_Result> GetDashboardStatsForEmail(double ThresholdValue,DateTime? RequestDate, bool? IsManagmentInclude)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var results = dbContext.sp_GetDashboardStats1(RequestDate, IsManagmentInclude).ToList();
                    if(results != null && results.Count > 0)
                    {
                        return results;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public void WeeklyEmailValueUpdate(string value)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    dbContext.sp_WeeklyEmailUpdateValue(value);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public bool IsEmailAlreadySent(int iProjectID, DateTime dtEmailTime)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_EmailNotificationCheck1(iProjectID, dtEmailTime).SingleOrDefault();
                    if(result != null)
                    {
                        if(result.Response == 1)
                        {
                            return true;
                        }
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void AddNotification(int iProjectID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_AddNotification(iProjectID);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

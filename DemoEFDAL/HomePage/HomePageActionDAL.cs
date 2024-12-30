using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.HomePage
{
    public class HomePageActionDAL : IHomePageAction
    {
        public List<sp_GetDashboardStats1_Result> GetDashBoardStats(DateTime RequestDate, bool IsManagementTimeExclude)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    // need to check
                    var result = dbContext.sp_GetDashboardStats1(RequestDate, IsManagementTimeExclude).ToList();
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

        public List<sp_GetDashboardStats_Staff1_Result> GetDashBoardStatsForStaff(DateTime dtRequired, string iStaffID)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    // need to check
                    var result = dbContext.sp_GetDashboardStats_Staff(dtRequired, iStaffID.ToString()).ToList();
                    if(result != null)
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

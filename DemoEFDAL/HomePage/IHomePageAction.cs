using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.HomePage
{
    public interface IHomePageAction
    {
        List<sp_GetDashboardStats_Staff1_Result> GetDashBoardStatsForStaff(DateTime DtRequired, string iStaffID);

        List<sp_GetDashboardStats1_Result> GetDashBoardStats(DateTime RequestDate, bool IsManagementTimeExclude);

    }
}

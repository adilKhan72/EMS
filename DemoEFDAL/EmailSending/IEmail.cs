using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.EmailSending
{
    public interface IEmail
    {
        List<sp_GetDashboardStats1_Result> GetDashboardStatsForEmail(double ThresholdValue, DateTime? RequestDate, bool? IsManagmentInclude);

        void WeeklyEmailValueUpdate(string value);

        bool IsEmailAlreadySent(int iProjectID, DateTime dtEmailTime);

        void AddNotification(int iProjectID);


    }
}

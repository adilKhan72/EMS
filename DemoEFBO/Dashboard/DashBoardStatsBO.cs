using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Dashboard
{
    //public class GetDashBoardStatsBO
    //{
    //    public string Year { get; set; }
    //    public string Month { get; set; }
    //    public string ManagementTime { get; set; } 
    //}
    public class GetDashBoardStatsUpdateBO
    {
        public DateTime? RequestStartDate { get; set; }
        public DateTime? RequestEndDate { get; set; }
        public int ProjectID { get; set; }
        public bool  ManagementCheck { get; set; }
    }
    public class GetDashBoardStatsForStaffBO
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string iStaffID { get; set; }
    }

    public class GetDashBoardStatsForStaffSearchBO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string iStaffID { get; set; }
    }

    public class GetDashBoardStatsBODAL
    {
        public DateTime? dtRequired { get; set; }
        public bool _IsManagementTimeExclude { get; set; }
    }

    public class DashBoardStatsBO
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string TotalDaysSpend { get; set; }
        public string TotalEstimatedDays { get; set; }
        public double CompletedPercentage { get; set; }
        public bool IsLimitExceed { get; set; }
    }

    public class DashBoardStatsUpdateBO
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool isActive { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectOwner { get; set; }
        public string ProjectType { get; set; }
        public string TotalDaysSpend { get; set; }
        public string TotalEstimatedDays { get; set; }
        public double CompletedPercentage { get; set; }
        public bool IsLimitExceed { get; set; }
        public int ClientID { get; set; }
        public decimal Total_Hours_Spent { get; set; }
        public string ClientName { get; set; }

    }
}

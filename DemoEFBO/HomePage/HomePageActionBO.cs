using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.HomePage
{
    public class HomePageActionBO
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string TotalDaysSpend { get; set; }
        public string TotalEstimatedDays { get; set; }
        public double CompletedPercentage { get; set; }
        public bool IsLimitExceed { get; set; }
    }
}

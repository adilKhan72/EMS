using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Reports
{
    public class ActualVsEstimatedComparisonReportModelBO
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string TaskName { get; set; }
        public float ActualHours { get; set; }
        public float EstimatedDuration { get; set; }
        public float RemainingHours { get; set; }
        public string OverBudget { get; set; }
    }
}

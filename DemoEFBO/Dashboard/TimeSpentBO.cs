using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Dashboard
{
    public class TimeSpenBO
    {
        public int ProjectId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class TimeSubtaskBO
    {
        public int ProjectId { get; set; }
        public string MaintaskName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class TestResponse
    {
        public List<string> MainTaskNames { get; set; }
        public List<decimal> HoursArr { get; set; }
        public List<decimal> DayArr { get; set; }
    }

    public class SubTaskTimeResponse
    {
        public List<string> SubTaskNames { get; set; }
        public List<decimal> HoursArr { get; set; }
        public List<decimal> DayArr { get; set; }
    }

    public class SubTaskTimeOwnerResponse
    {
        public string TaskName { get; set; }
        public string TaskOwnerName { get; set; }

    }
}

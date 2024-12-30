using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Reports
{
    public class TotalHoursOfProjectParamBO
    {
        public int ProjectId { get; set; }
        public string TaskName { get; set; }
        public int TaskOwnerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int MainTaskID { get; set; }
        public string Approved { get; set; }
        public int TaskType { get; set; }
    }
}

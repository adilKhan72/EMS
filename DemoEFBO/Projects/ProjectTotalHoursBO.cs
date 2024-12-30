using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Projects
{
    public class ProjectTotalHoursBO
    {
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal? TotalActualDuration { get; set; }
        public decimal? RemainingHours { get; set; }
    }
}

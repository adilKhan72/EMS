using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.ProjectSchedule
{
    public class ExcelMappingBO
    {
        public string ProjectType { get; set; }
        public string ProjectName { get; set; }
        public string ResourceName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Estimate { get; set; }
        public string ProjectEstimate { get; set; }
        public List<ProjectScheduleUpdateBO> lstProUpdate { get; set; }
        public string ProjectComment { get; set; }
    }
}

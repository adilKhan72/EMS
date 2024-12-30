using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.ProjectSchedule
{
    public class ProjectResourceExportBO
    {
        public string ResourceName { get; set; }
        public int ResourceDetailID { get; set; }
        public int ResourceID { get; set; }
        public string Phase { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Duration { get; set; }
        public int ProjectID { get; set; }
    }
}

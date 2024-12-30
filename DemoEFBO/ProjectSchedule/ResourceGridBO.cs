using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.ProjectSchedule
{
    public class ResourceGridBO
    {
        public int ResourceDetailID { get; set; }
        public int ResourceId { get; set; }
        public int projectID { get; set; }
        public string Department { get; set; }
        public string ResourceName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Duration { get; set; }
    }
}

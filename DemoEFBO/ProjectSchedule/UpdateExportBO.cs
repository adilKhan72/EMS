using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.ProjectSchedule
{
    public class UpdateExportBO
    {
        public string FullName { get; set; }
        public int UpdateID { get; set; }
        public string UpdateNumber { get; set; }
        public int ResourceID { get; set; }
        public int ProjectID { get; set; }
        public string Comments { get; set; }
        public string Phase { get; set; }
    }
}

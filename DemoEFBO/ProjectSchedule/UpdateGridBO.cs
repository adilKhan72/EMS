using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.ProjectSchedule
{
    public class UpdateGridBO
    {
        public int UpdateID { get; set; }
        public int ResourceID { get; set; }
        public int ProjectID { get; set; }
        public string UpdateNumber { get; set; }
        public string ResourceName { get; set; }
        public string Comments { get; set; }
        public string phase { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Tasks
{
    public class TaskDescriptionBO
    {
            public int projectID { get; set; }
            public int maintaskID { get; set; }

    }
    public class TaskDescriptionResponseBO
    {
        public int taskTableID { get; set; }
        public string taskName { get; set; }
        public int projectID { get; set; }
        public int maintaskID { get; set; }
        public int isActive { get; set; }
        public int estimatedDurations { get; set; }
        public int taskTypeId { get; set; }
        public int phase { get; set; }
        

    }
}

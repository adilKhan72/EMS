using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Tasks
{
    public class SubTasksBO
    {
        public int projectID { get; set; }
        public int maintaskID { get; set; }
    }
    public class SubTasksResponseBO
    {
        public int SubtaskId { get; set; }
        public string TaskName { get; set; }
    }
}

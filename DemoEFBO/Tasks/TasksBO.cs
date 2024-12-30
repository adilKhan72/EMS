using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Tasks
{
    public class TasksBO
    {
        public int ID { get; set; }
        public string TaskName { get; set; }
        public bool IsActive { get; set; }
        public decimal EstimatedDuration { get; set; }
        public int TaskTypeId { get; set; }
        public string TaskTypeName { get; set; }
        public string TaskTypePrefix { get; set; }
        public int Phase { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string MainTask { get; set; }
        public int MainTaskID { get; set; }
        public string Comments { get; set; }
    }
}

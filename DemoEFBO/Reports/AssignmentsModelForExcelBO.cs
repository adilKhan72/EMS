using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Reports
{
    public class AssignmentsModelForExcelBO
    {
        public int ID { get; set; }
        public string Project { get; set; }
        public int ProjectID { get; set; }
        public string Date { get; set; }
        public string TaskOwner { get; set; }
        public string Task { get; set; }
        public float Duration { get; set; }
        public decimal Days { get; set; }
        public decimal RemainingHours { get; set; }
        public bool IsApproved { get; set; }
        public string TaskTypePrefix { get; set; }
        public string TaskType { get; set; }
        public string Phase { get; set; }
        public string ProjectNameForWeek { get; set; }
        public string SubTask { get; set; }
        public int MainTaskID { get; set; }
        public string MainTaskName { get; set; }
    }
}

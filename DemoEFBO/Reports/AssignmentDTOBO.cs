using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Reports
{
    public class AssignmentDTOBO
    {
        public int LoggedInUserId { get; set; }
        public int ProjectId { get; set; }
        public string TaskName { get; set; }
        public int TaskOwnerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string[] taskTypePrefixes { get; set; }
        public bool? IsApproved { get; set; }
        public int MainTaskID { get; set; }
    }
 

    public class ExportAssignmentBO
    {
        public int ProjectId { get; set; }
        public int[] TaskOwnerId { get; set; }
        public string commasepartedTaskOwnerids { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int TaskId { get; set; }
        public bool? Billable { get; set; }
        public bool? SettingDateRange { get; set; }
        public string fileName { get; set; }
        public bool IsZeroRowShow { get; set; }
        public bool IsDifferencehourse { get; set; }
        public List<ProjectAllArray> AllArrays { get; set; }
        public List<ClientAllArray> ClientArray { get; set; }
        public int SubTaskId { get; set; }
        public int ClinetID { get; set; }
        public string pdftextboxvalue { get; set; }
        public bool ExportType { get; set; }
        public string projectwise { get;set; }
        public int DepartmentID { get; set; }
        public bool ShowSubTask { get; set; }

    }
    public class ProjectAllArray
    {
        public int ProjectIdAll { get; set; }
        public string ProjectNameAll { get; set; }
        public string ReferenceNumber { get; set; }
        public int ClientID { get; set; }
    }
    public class ClientAllArray
    {
        public string ClientName { get; set; }
        public int ID { get; set; }
    }
}

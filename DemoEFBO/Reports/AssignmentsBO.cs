using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Reports
{
    public class AssignmentsBO
    {
        public int AssignmentID { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectID { get; set; }
        public string MainTaskName { get; set; }
        public int MainTaskID { get; set; }
        public string TaskName { get; set; }
        public int TaskID { get; set; }
        public string ProjectDescription { get; set; }
        public string UserFullName { get; set; }
        public int UserID { get; set; }
        public string AssignmentDateTime { get; set; }
        public string DaysDuration { get; set; }
        public string ActualDuration { get; set; }
        public string CommentText { get; set; }
        public int IsApproved { get; set; }
        public string BillableHours { get; set; }
        public int? IsBillableApproved { get; set; }
        public int? UserIDBillable { get; set; }
        public string UserNameBillable { get; set; }
        public DateTime? BillableDateTime { get; set; }
        public int? IsActualApproved { get; set; }
        public int? UserIDActual { get; set; }
        public string UserNameActual { get; set; }
        public DateTime? ActualDateTime { get; set; }
        public int ClientID { get; set; }
        public string week { get; set; }
        public string hours { get; set; }
        public string Range { get; set; }
        public string Days { get; set; }
        public bool Rowentry { get; set; }
        public bool CheckIsApproved { get; set; }
        public DateTime AssignmentDT { get; set; }
        public string Billable_Days { get; set; }
        public string Billable_Hours { get; set; }
        public string RowDescription { get; set; }
    }

    public class AddRecordBO
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public int TASKID { get; set; }
        public int UserID { get; set; }
        public string AssignmentDateTime { get; set; }
        public string ActualDuration { get; set; }
        public string CommentText { get; set; }
        public string BillableHours { get; set; }
        public int IsBillableApproved { get; set; }
        public int UserIDBillable { get; set; }
        public int IsActualApproved { get; set; }
        public int UserIDActual { get; set; }
        public int MainTaskID { get; set; }


    }

    public class GetAssignmentBO
    {
        public int[] LoggedInUserId { get; set; }
        public int ProjectId { get; set; }
        public int SubTaskID { get; set; }
        public int MainTaskID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsApproved { get; set; }
        public int ClientID { get; set; }
        public int DepartmentID { get; set; }
    }
    public class ProjectTotalHoursViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string TotalActualDuration { get; set; }
        public string BillableHours { get; set; }
        public string RemainingHours { get; set; }
        public int ClientID { get; set; }
        public Nullable<decimal> ActualD { get; set; }
    }
    public class AssignmentDTO
    {
        public int LoggedInUserId { get; set; }
        public int ProjectId { get; set; }
        public string TaskName { get; set; }
        public int[] TaskOwnerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string[] taskTypePrefixes { get; set; }
        public bool? IsApproved { get; set; }
        public int MainTaskID { get; set; }
        public int ClientID { get; set; }
        public int SubTaskID { get; set; }
        public int DepartmentID { get; set; }
        //public string MainTaskName { get; set; }
    }

}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DemoEFDAL
{
    using System;
    
    public partial class sp_GetTasksNotFound_Result
    {
        public int ID { get; set; }
        public string ProjectName { get; set; }
        public string TaskDescription { get; set; }
        public string TaskOwnerName { get; set; }
        public Nullable<System.DateTime> AssignmentDateTime { get; set; }
        public int ProjectId { get; set; }
        public Nullable<decimal> ActualDuration { get; set; }
        public int TaskOwnerId { get; set; }
        public string SubTaskDescription { get; set; }
    }
}

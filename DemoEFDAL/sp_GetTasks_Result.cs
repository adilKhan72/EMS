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
    
    public partial class sp_GetTasks_Result
    {
        public int ID { get; set; }
        public string TaskName { get; set; }
        public Nullable<decimal> EstimatedDuration { get; set; }
        public string Comments { get; set; }
        public Nullable<int> Phase { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> TaskTypeId { get; set; }
        public string TaskTypeName { get; set; }
        public string TaskTypePrefix { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string MainTask { get; set; }
        public int MainTaskId { get; set; }
    }
}

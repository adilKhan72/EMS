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
    
    public partial class sp_GetRecurringTasks_Result
    {
        public int ID { get; set; }
        public string TaskName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<decimal> EstimatedDuration { get; set; }
        public Nullable<int> TaskTypeId { get; set; }
        public Nullable<int> Phase { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> MainTaskId { get; set; }
    }
}

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
    
    public partial class sp_GetProjectsLazyLoading_Result
    {
        public Nullable<long> RowNum { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsExternal { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Type { get; set; }
        public Nullable<decimal> EstimatedHours { get; set; }
        public string ProjectDescription { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string ProjectOwner { get; set; }
    }
}

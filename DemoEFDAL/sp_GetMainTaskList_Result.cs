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
    
    public partial class sp_GetMainTaskList_Result
    {
        public int ID { get; set; }
        public string MainTaskName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string ProjectIDs { get; set; }
        public string ProjectType { get; set; }
    }
}

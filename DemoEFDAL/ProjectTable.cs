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
    using System.Collections.Generic;
    
    public partial class ProjectTable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProjectTable()
        {
            this.TaskTableWithEstimates = new HashSet<TaskTableWithEstimate>();
            this.TaskTables = new HashSet<TaskTable>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsExternal { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Type { get; set; }
        public Nullable<decimal> EstimatedHours { get; set; }
        public Nullable<decimal> ContractHoursPerMonth { get; set; }
        public Nullable<decimal> ContractHoursInJanuary { get; set; }
        public Nullable<decimal> ContractHoursInFebruary { get; set; }
        public Nullable<decimal> ContractHoursInMarch { get; set; }
        public Nullable<decimal> ContractHoursInApril { get; set; }
        public Nullable<decimal> ContractHoursInMay { get; set; }
        public Nullable<decimal> ContractHoursInJune { get; set; }
        public Nullable<decimal> ContractHoursInJuly { get; set; }
        public Nullable<decimal> ContractHoursInAugust { get; set; }
        public Nullable<decimal> ContractHoursInSeptember { get; set; }
        public Nullable<decimal> ContractHoursInOctober { get; set; }
        public Nullable<decimal> ContractHoursInNovember { get; set; }
        public Nullable<decimal> ContractHoursInDecember { get; set; }
        public string ProjectDescription { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string ProjectOwner { get; set; }
        public Nullable<int> ClientID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaskTableWithEstimate> TaskTableWithEstimates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaskTable> TaskTables { get; set; }
    }
}

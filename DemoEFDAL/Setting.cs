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
    
    public partial class Setting
    {
        public int iSettingId { get; set; }
        public string vSettingName { get; set; }
        public string vSettingValue { get; set; }
        public Nullable<System.DateTime> dtSettingCreation { get; set; }
        public Nullable<System.DateTime> dtSettingModification { get; set; }
        public string UserName { get; set; }
        public string lastModifiedBy { get; set; }
        public string SettingType { get; set; }
        public string Setting_Description { get; set; }
    }
}

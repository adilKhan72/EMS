using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Setting
{
    public class SettingRequestBO
    {
        public string SettingName { get; set; }
    }
    public class SettingModelBO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string CreationDate { get; set; }
        public string ModificationDate { get; set; }
        public string UserName { get; set; }
        public string Mode { get; set; }
        public string LastModifiedBy { get; set; }
        public string SettingType { get; set; }
        public string SettingDescription { get; set; }


    }
}

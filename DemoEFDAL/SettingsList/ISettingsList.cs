using DemoEFBO.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.SettingsList
{
    interface ISettingsList
    {
        List<GetSetting_Result> GetSettingsDAL();

        sp_SettingListWithParam_Result GetSettingsParamDAL(string SettingName);

        //List<GetSetting_Result> GetSetting(string SettingName);

        bool InsertSetting(SettingModelBO obj);

        bool DeleteSetting(int id, bool mode);
    }
}

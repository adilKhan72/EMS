using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.Setting;

namespace DemoEFDAL.SettingsList
{
    public class SettingsDAL : ISettingsList
    {
        // For Fetching All settings
        public List<GetSetting_Result> GetSettingsDAL()
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var lst = dbContext.GetSetting("").ToList(); // sp_SettingList().ToList();
                    if (lst != null)
                    {
                        return lst;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex) { return null;}
        }

        public sp_SettingListWithParam_Result GetSettingsParamDAL(string SettingName)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_SettingListWithParam1(SettingName).SingleOrDefault();
                    if(result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<GetSetting_Update_Result> GetSetting(string SettingName)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.GetSetting_Update(SettingName).ToList();
                    if(result != null && result.Count > 0)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool InsertSetting(SettingModelBO obj)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_InsertSetting_Update(Convert.ToBoolean(obj.Mode), obj.Id, obj.Name, obj.Value, obj.UserName,obj.SettingType,obj.SettingDescription);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteSetting(int id, bool mode)
        {
            try
            {
                using (var dbContext = new EMSEntities())
                {
                    var result = dbContext.sp_InsertSetting(mode, id, null,null,null);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

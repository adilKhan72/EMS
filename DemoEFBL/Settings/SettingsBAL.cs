using DemoEFBO.Setting;
using DemoEFDAL.SettingsList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.Settings
{
    public class SettingsBAL
    {
        public List<SettingBO> GetSettings()
        {
            SettingsDAL obj = new SettingsDAL();
            List<SettingBO> lst = new List<SettingBO>();
            try
            {
                var result = obj.GetSettingsDAL();
                if(result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new SettingBO
                        {
                            iSettingID = result[i].iSettingId,
                            vSettingName = result[i].vSettingName,
                            vSettingValue = result[i].vSettingValue
                        });
                    }
                    return lst;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public SettingBO GetSettingWithParam(string SettingName)
        {
            try
            {
                if(!string.IsNullOrEmpty(SettingName))
                {
                    SettingsDAL objDAL = new SettingsDAL();
                    SettingBO objData = new SettingBO();
                    var result = objDAL.GetSettingsParamDAL(SettingName);
                    if(result != null)
                    {
                        objData.iSettingID = result.iSettingId;
                        objData.vSettingName = result.vSettingName;
                        objData.vSettingValue = result.vSettingValue;
                        return objData;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetSettingValue(List<SettingBO> lst, string value)
        {
            try
            {
                var _val = lst.Find(x => x.vSettingName == value);
                return _val.vSettingValue;
            }
            catch { return ""; }
        }

        public List<SettingModelBO> GetSetting(SettingRequestBO objSetting)
        {
            try
            {
                var settings = new List<SettingModelBO>();
                SettingsDAL objDal = new SettingsDAL();
                string SettingName = objSetting == null ? "" : objSetting.SettingName;
                var result = objDal.GetSetting(SettingName);
                if(result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        settings.Add(new SettingModelBO
                        {
                            Id = result[i].iSettingId,
                            Name = result[i].vSettingName,
                            Value = result[i].vSettingValue,
                            CreationDate = result[i].dtSettingCreation,
                            ModificationDate = result[i].dtSettingModification,
                            UserName = result[i].UserName,
                            SettingType = result[i].SettingType,
                            SettingDescription = result[i].Setting_Description,
                           LastModifiedBy = result[i].lastModifiedBy,
                        });
                    }
                    return settings;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public bool InsertSetting(SettingModelBO setting)
        {
            try
            {
                if(setting != null)
                {
                    SettingsDAL objDal = new SettingsDAL();
                    return objDal.InsertSetting(setting);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteSetting(int ID)
        {
            try
            {
                if(ID > 0)
                {
                    SettingsDAL objDal = new SettingsDAL();
                    return objDal.DeleteSetting(ID,true);
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }





    }
}

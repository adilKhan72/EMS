using DemoEFBO.HomePage;
using DemoEFDAL.HomePage;
using DemoEFBL.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.HomePage
{
    public class HomePageActionsBL
    {
        // For Getting Dashboard Stats in case of Admin Login for Projects
        public List<HomePageActionBO> GetDashBoardStats(string Year, String Month, string ManagementTime)
        {
            try
            {
                if(!string.IsNullOrEmpty(Year) && !string.IsNullOrEmpty(Month) && !string.IsNullOrEmpty(ManagementTime))
                {
                    List<HomePageActionBO> lst = new List<HomePageActionBO>();
                    bool _IsManagementTimeExclude = false;
                    if (!string.IsNullOrEmpty(ManagementTime))
                    {
                        if (ManagementTime == "1")
                        {
                            _IsManagementTimeExclude = true;
                        }
                    }
                    DateTime dtRequired = DateTime.Now;
                    if (!string.IsNullOrEmpty(Year) &&
                        !string.IsNullOrEmpty(Month) &&
                        Year != "-1" &&
                        Month != "-1"
                       )
                    {
                        int _year = 0; int _month = 0;
                        int.TryParse(Year, out _year);
                        int.TryParse(Month, out _month);
                        dtRequired = new DateTime(_year, _month, DateTime.Now.Day);
                    }
                    SettingsBAL objSettingBAL = new SettingsBAL();
                    int _threshold = 80; // default value
                    var objSetting = objSettingBAL.GetSettingWithParam("ProjectCompletionThresold");
                    if(objSetting != null)
                    {
                        _threshold = Convert.ToInt32(objSetting.vSettingValue);
                    }
                    HomePageActionDAL objDAL = new HomePageActionDAL();
                    var result = objDAL.GetDashBoardStats(dtRequired, _IsManagementTimeExclude);
                    if(result != null && result.Count > 0)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            double _percentage, _TotalDaysSpend, _ContractHours = 0;
                            double.TryParse(result[i].Total_Hours_Spent.ToString(), out _TotalDaysSpend);
                            double.TryParse(result[i].ContractHoursPerMonth.ToString(), out _ContractHours);
                            bool _isLimit = false;
                            _TotalDaysSpend = _TotalDaysSpend / 8;// converting into days
                            _ContractHours = _ContractHours / 8; // converting into days
                            _percentage = (_TotalDaysSpend / _ContractHours) * 100;
                            if (_percentage >= _threshold)
                            {
                                _isLimit = true;
                            }
                            if (_ContractHours > 0)
                            {
                                lst.Add(new HomePageActionBO
                                {
                                    ProjectID = result[i].ID,
                                    ProjectName = result[i].Name,
                                    TotalDaysSpend = _TotalDaysSpend.ToString("F"),
                                    TotalEstimatedDays = _ContractHours.ToString("F"),
                                    CompletedPercentage = Math.Round(Convert.ToDouble(_percentage.ToString("F")), 0, MidpointRounding.AwayFromZero),
                                    IsLimitExceed = _isLimit
                                });
                            }
                        }
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

        // return dashboard stats against staff
        public List<HomePageActionBO> GetDashBoardStatsForStaff(string Year, String Month, string iStaffID)
        {
            try
            {
                if (!string.IsNullOrEmpty(Year) && !string.IsNullOrEmpty(Month) && !string.IsNullOrEmpty(iStaffID))
                {
                    int _StaffID = -1;
                    int.TryParse(iStaffID, out _StaffID);
                    DateTime dtRequired = DateTime.Now;
                    if (!string.IsNullOrEmpty(Year) &&
                        !string.IsNullOrEmpty(Month) &&
                        Year != "-1" &&
                        Month != "-1"
                        )
                    {
                        int _year = 0; int _month = 0;
                        int.TryParse(Year, out _year);
                        int.TryParse(Month, out _month);
                        dtRequired = new DateTime(_year, _month, DateTime.Now.Day);
                    }
                    HomePageActionDAL objDal = new HomePageActionDAL();
                    List<HomePageActionBO> lst = new List<HomePageActionBO>();
                    var result = objDal.GetDashBoardStatsForStaff(dtRequired, _StaffID.ToString());
                    if(result != null && result.Count > 0)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            double _percentage, _TotalDaysSpend, _TotalProjectHours = 0;
                            double.TryParse(result[i].Total_Hours_Spent.ToString(), out _TotalDaysSpend);
                            double.TryParse(result[i].TotalProjectHour.ToString(), out _TotalProjectHours);
                            _TotalDaysSpend = _TotalDaysSpend / 8;// converting into days
                            _TotalProjectHours = _TotalProjectHours / 8; // converting into days
                            _percentage = (_TotalDaysSpend / _TotalProjectHours) * 100;

                            if (_TotalProjectHours > 0)
                            {
                                lst.Add(new HomePageActionBO
                                {
                                    ProjectID = result[i].ID,
                                    ProjectName = result[i].Name,
                                    TotalDaysSpend = _TotalDaysSpend.ToString("F"),
                                    TotalEstimatedDays = _TotalProjectHours.ToString("F"),
                                    CompletedPercentage = Math.Round(Convert.ToDouble(_percentage.ToString("F")), 0, MidpointRounding.AwayFromZero),
                                    IsLimitExceed = false
                                });
                            }
                        }
                        return lst;
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

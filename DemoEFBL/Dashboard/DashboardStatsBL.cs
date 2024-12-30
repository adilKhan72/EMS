using DemoEFBL.Settings;
using DemoEFBO.Dashboard;
using DemoEFDAL.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.Dashboard
{
    public class DashboardStatsBL
    {
        //public List<DashBoardStatsBO> GetDashBoardStatsBL(GetDashBoardStatsBO objGetState)
        //{
        //    try
        //    {
        //        SettingsBAL objSettingBL = new SettingsBAL();
        //        var _listSetting = objSettingBL.GetSettings();
        //        var _response = _listSetting.Find(x => x.vSettingName == "ProjectCompletionThresold");
        //        double _thresholdValue = 0;
        //        double.TryParse(_response.vSettingValue, out _thresholdValue);

        //        bool _IsManagementTimeExclude = false;
        //        if (!string.IsNullOrEmpty(objGetState.ManagementTime))
        //        {
        //            if (objGetState.ManagementTime == "1")
        //            {
        //                _IsManagementTimeExclude = true;
        //            }
        //        }

        //        DateTime dtRequired = DateTime.Now;
        //        if (!string.IsNullOrEmpty(objGetState.Year) &&
        //            !string.IsNullOrEmpty(objGetState.Month) &&
        //            objGetState.Year != "-1" &&
        //            objGetState.Month != "-1"
        //           )
        //        {
        //            int _year = 0; int _month = 0;
        //            int.TryParse(objGetState.Year, out _year);
        //            int.TryParse(objGetState.Month, out _month);
        //            dtRequired = new DateTime(_year, _month, DateTime.Now.Day);
        //        }


        //        List<DashBoardStatsBO> DashBoardStatsResponse = new List<DashBoardStatsBO>();

        //        DashboardStatsDAL ObjDAL = new DashboardStatsDAL();

        //        var objResult = ObjDAL.GetDashboardStatsDAL(dtRequired, _IsManagementTimeExclude);

        //        if (objResult != null)
        //        {
        //            for (int i = 0; i < objResult.Count; i++)
        //            {
        //                double _percentage, _TotalDaysSpend, _ContractHours = 0;
        //                double.TryParse(objResult[i].Total_Hours_Spent.ToString(), out _TotalDaysSpend);
        //                double.TryParse(objResult[i].ContractHoursPerMonth.ToString(), out _ContractHours);
        //                bool _isLimit = false;
        //                _TotalDaysSpend = _TotalDaysSpend / 8;// converting into days
        //                _ContractHours = _ContractHours / 8; // converting into days
        //                _percentage = (_TotalDaysSpend / _ContractHours) * 100;

        //                if (_percentage >= _thresholdValue)
        //                {
        //                    _isLimit = true;
        //                }

        //                if (_ContractHours > 0)
        //                {
        //                    DashBoardStatsResponse.Add(new DashBoardStatsBO
        //                    {
        //                        ProjectID = Convert.ToInt32(objResult[i].ID),
        //                        ProjectName = objResult[i].Name.ToString(),
        //                        TotalDaysSpend = _TotalDaysSpend.ToString("F"),
        //                        TotalEstimatedDays = _ContractHours.ToString("F"),
        //                        CompletedPercentage = Math.Round(Convert.ToDouble(_percentage.ToString("F")), 0, MidpointRounding.AwayFromZero),
        //                        IsLimitExceed = _isLimit
        //                    });
        //                }

        //            }
        //            return DashBoardStatsResponse;
        //        }
        //        return null;

        //    }

        //    catch (Exception e) { return null; }
        //}

        public List<DashBoardStatsUpdateBO> GetDashBoardStatsUpdateBL(GetDashBoardStatsUpdateBO objGetState)
        {
            try
            {
                SettingsBAL objSettingBL = new SettingsBAL();
                var _listSetting = objSettingBL.GetSettings();
                var _response = _listSetting.Find(x => x.vSettingName == "ProjectCompletionThresold");
                double _thresholdValue = 0;
                double.TryParse(_response.vSettingValue, out _thresholdValue);

                var _IsManagementTimeresponse = _listSetting.Find(x => x.vSettingName == "IsManagementTime");

                bool IsManagementTime = false;
                if (_IsManagementTimeresponse != null)
                {
                    bool.TryParse(_IsManagementTimeresponse.vSettingValue, out IsManagementTime);
                }


                List<DashBoardStatsUpdateBO> DashBoardStatsResponse = new List<DashBoardStatsUpdateBO>();

                DashboardStatsDAL ObjDAL = new DashboardStatsDAL();

                var objResult = ObjDAL.GetDashboardStatsUpdateDAL(objGetState.RequestStartDate, objGetState.RequestEndDate, objGetState.ProjectID, objGetState.ManagementCheck);

             
                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        if (string.IsNullOrEmpty(objResult[i].StartDate.ToString())){
                            objResult[i].StartDate = DateTime.Today;
                        }
                        if (string.IsNullOrEmpty(objResult[i].EndDate.ToString()))
                        {
                            objResult[i].EndDate = DateTime.Today;
                        }
                        if (string.IsNullOrEmpty(objResult[i].Name))
                        {
                            objResult[i].Name = "";
                        }
                        if (string.IsNullOrEmpty(objResult[i].ProjectDescription))
                        {
                            objResult[i].ProjectDescription = "";
                        }
                        if (string.IsNullOrEmpty(objResult[i].ProjectOwner))
                        {
                            objResult[i].ProjectOwner = "";
                        }
                        if (string.IsNullOrEmpty(objResult[i].Type))
                        {
                            objResult[i].Type = "";
                        }




                        double _percentage, _TotalDaysSpend, _ContractHours = 0;
                        double.TryParse(objResult[i].Total_Hours_Spent.ToString(), out _TotalDaysSpend);
                        double.TryParse(objResult[i].ContractHoursPerMonth.ToString(), out _ContractHours);
                        bool _isLimit = false;
                        _TotalDaysSpend = _TotalDaysSpend / 8;// converting into days
                        _ContractHours = _ContractHours / 8; // converting into days
                        _percentage = (_TotalDaysSpend / _ContractHours) * 100;

                        if (_percentage >= _thresholdValue)
                        {
                            _isLimit = true;
                        }
                        if (Double.IsInfinity(_percentage))
                        {
                            _percentage = (_TotalDaysSpend / _TotalDaysSpend) * 100;
                        }
                        if (Double.IsNaN(_percentage))
                        {
                            _percentage = 0;
                        }

                        //if (_ContractHours > 0)
                        //{
                            DashBoardStatsResponse.Add(new DashBoardStatsUpdateBO
                            {
                                ProjectID = Convert.ToInt32(objResult[i].ID),
                                ProjectName = objResult[i].Name.ToString(),
                                TotalDaysSpend = _TotalDaysSpend.ToString("F"),
                                TotalEstimatedDays = _ContractHours.ToString("F"),
                                CompletedPercentage = Math.Round(Convert.ToDouble(_percentage.ToString("F")), 0, MidpointRounding.AwayFromZero),
                                IsLimitExceed = _isLimit,
                                StartDate = objResult[i].StartDate.ToString(),
                                EndDate = objResult[i].EndDate.ToString(),
                                isActive = Convert.ToBoolean(objResult[i].IsActive),
                                ProjectDescription = objResult[i].ProjectDescription.ToString(),
                                ProjectOwner = objResult[i].ProjectOwner.ToString(),
                                ProjectType = objResult[i].Type.ToString(),
                                ClientID= Convert.ToInt32(objResult[i].ClientID),
                                Total_Hours_Spent = Convert.ToDecimal(objResult[i].Total_Hours_Spent),
                            });
                        //}

                    }
                    return DashBoardStatsResponse.OrderBy(a => a.ProjectName).ToList();
                }
                return null;

            }

            catch (Exception e) { return null; }
        }

        public List<DashBoardStatsUpdateBO> GetAll_Project(GetDashBoardStatsUpdateBO objGetState)
        {
            try
            {
                SettingsBAL objSettingBL = new SettingsBAL();
                var _listSetting = objSettingBL.GetSettings();
                var _response = _listSetting.Find(x => x.vSettingName == "ProjectCompletionThresold");
                double _thresholdValue = 0;
                double.TryParse(_response.vSettingValue, out _thresholdValue);

                var _IsManagementTimeresponse = _listSetting.Find(x => x.vSettingName == "IsManagementTime");

                bool IsManagementTime = false;
                if (_IsManagementTimeresponse != null)
                {
                    bool.TryParse(_IsManagementTimeresponse.vSettingValue, out IsManagementTime);
                }


                List<DashBoardStatsUpdateBO> DashBoardStatsResponse = new List<DashBoardStatsUpdateBO>();

                DashboardStatsDAL ObjDAL = new DashboardStatsDAL();

                var objResult = ObjDAL.GetAllProjects(objGetState.ProjectID);


                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        if (string.IsNullOrEmpty(objResult[i].StartDate.ToString()))
                        {
                            objResult[i].StartDate = DateTime.Today;
                        }
                        if (string.IsNullOrEmpty(objResult[i].EndDate.ToString()))
                        {
                            objResult[i].EndDate = DateTime.Today;
                        }
                        if (string.IsNullOrEmpty(objResult[i].Name))
                        {
                            objResult[i].Name = "";
                        }
                        if (string.IsNullOrEmpty(objResult[i].ProjectDescription))
                        {
                            objResult[i].ProjectDescription = "";
                        }
                        if (string.IsNullOrEmpty(objResult[i].ProjectOwner))
                        {
                            objResult[i].ProjectOwner = "";
                        }
                        if (string.IsNullOrEmpty(objResult[i].Type))
                        {
                            objResult[i].Type = "";
                        }




                        double _percentage, _TotalDaysSpend, _ContractHours = 0;
                        double.TryParse(objResult[i].Total_Hours_Spent.ToString(), out _TotalDaysSpend);
                        double.TryParse(objResult[i].ContractHoursPerMonth.ToString(), out _ContractHours);
                        bool _isLimit = false;
                        _TotalDaysSpend = _TotalDaysSpend / 8;// converting into days
                        _ContractHours = _ContractHours / 8; // converting into days
                        _percentage = (_TotalDaysSpend / _ContractHours) * 100;

                        if (_percentage >= _thresholdValue)
                        {
                            _isLimit = true;
                        }

                        //if (_ContractHours > 0)
                        //{
                            DashBoardStatsResponse.Add(new DashBoardStatsUpdateBO
                            {
                                ProjectID = Convert.ToInt32(objResult[i].ID),
                                ProjectName = objResult[i].Name.ToString(),
                                TotalDaysSpend = _TotalDaysSpend.ToString("F"),
                                TotalEstimatedDays = _ContractHours.ToString("F"),
                                CompletedPercentage = Math.Round(Convert.ToDouble(_percentage.ToString("F")), 0, MidpointRounding.AwayFromZero),
                                IsLimitExceed = _isLimit,
                                StartDate = objResult[i].StartDate.ToString(),
                                EndDate = objResult[i].EndDate.ToString(),
                                isActive = Convert.ToBoolean(objResult[i].IsActive),
                                ProjectDescription = objResult[i].ProjectDescription.ToString(),
                                ProjectOwner = objResult[i].ProjectOwner.ToString(),
                                ProjectType = objResult[i].Type.ToString(),
                                ClientID = Convert.ToInt32(objResult[i].ClientID),
                                Total_Hours_Spent = Convert.ToDecimal(objResult[i].Total_Hours_Spent),
                            });
                       // }

                    }
                    return DashBoardStatsResponse;
                }
                return null;

            }

            catch (Exception e) { return null; }
        }

        public List<DashBoardStatsBO> GetDashBoardStatsForStaffBL(GetDashBoardStatsForStaffBO objGetState)
        {
            try
            {
                int _StaffID = -1;
                int.TryParse(objGetState.iStaffID, out _StaffID);


                DateTime dtRequired = DateTime.Now;
                if (!string.IsNullOrEmpty(objGetState.Year) &&
                    !string.IsNullOrEmpty(objGetState.Month) &&
                    objGetState.Year != "-1" &&
                    objGetState.Month != "-1"
                   )
                {
                    int _year = 0; int _month = 0;
                    int.TryParse(objGetState.Year, out _year);
                    int.TryParse(objGetState.Month, out _month);
                    dtRequired = new DateTime(_year, _month, DateTime.Now.Day);
                }

                List<DashBoardStatsBO> DashBoardStatsResponse = new List<DashBoardStatsBO>();

                DashboardStatsDAL ObjDAL = new DashboardStatsDAL();

                var objResult = ObjDAL.GetDashboardStatsForStaffDAL(dtRequired, objGetState.iStaffID);

                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        double _percentage, _TotalDaysSpend, _TotalProjectHours = 0;
                        double.TryParse(objResult[i].Total_Hours_Spent.ToString(), out _TotalDaysSpend);
                        double.TryParse(objResult[i].TotalProjectHour.ToString(), out _TotalProjectHours);
                        bool _isLimit = false;
                        _TotalDaysSpend = _TotalDaysSpend / 8;// converting into days
                        _TotalProjectHours = _TotalProjectHours / 8; // converting into days
                        _percentage = (_TotalDaysSpend / _TotalProjectHours) * 100;


                        if (_TotalProjectHours > 0)
                        {
                            DashBoardStatsResponse.Add(new DashBoardStatsBO
                            {
                                ProjectID = Convert.ToInt32(objResult[i].ID),
                                ProjectName = objResult[i].Name.ToString(),
                                TotalDaysSpend = _TotalDaysSpend.ToString("F"),
                                TotalEstimatedDays = _TotalProjectHours.ToString("F"),
                                CompletedPercentage = Math.Round(Convert.ToDouble(_percentage.ToString("F")), 0, MidpointRounding.AwayFromZero),
                                IsLimitExceed = false
                            });
                        }

                    }
                    return DashBoardStatsResponse;
                }
                return null;
            }

            catch (Exception e) { return null; }
        }
        public List<DashBoardStatsUpdateBO> GetDashBoardStatsUpdateBL_ByProjectID(GetDashBoardStatsUpdateBO objGetState)
        {
            try
            {
                SettingsBAL objSettingBL = new SettingsBAL();
                var _listSetting = objSettingBL.GetSettings();
                var _response = _listSetting.Find(x => x.vSettingName == "ProjectCompletionThresold");
                double _thresholdValue = 0;
                double.TryParse(_response.vSettingValue, out _thresholdValue);

                var _IsManagementTimeresponse = _listSetting.Find(x => x.vSettingName == "IsManagementTime");

                bool IsManagementTime = false;
                if (_IsManagementTimeresponse != null)
                {
                    bool.TryParse(_IsManagementTimeresponse.vSettingValue, out IsManagementTime);
                }


                List<DashBoardStatsUpdateBO> DashBoardStatsResponse = new List<DashBoardStatsUpdateBO>();

                DashboardStatsDAL ObjDAL = new DashboardStatsDAL();

                var objResult = ObjDAL.GetProjectByID(objGetState.RequestStartDate, objGetState.RequestEndDate, objGetState.ProjectID, objGetState.ManagementCheck);


                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        if (string.IsNullOrEmpty(objResult[i].StartDate.ToString()))
                        {
                            objResult[i].StartDate = DateTime.Today;
                        }
                        if (string.IsNullOrEmpty(objResult[i].EndDate.ToString()))
                        {
                            objResult[i].EndDate = DateTime.Today;
                        }
                        if (string.IsNullOrEmpty(objResult[i].Name))
                        {
                            objResult[i].Name = "";
                        }
                        if (string.IsNullOrEmpty(objResult[i].ProjectDescription))
                        {
                            objResult[i].ProjectDescription = "";
                        }
                        if (string.IsNullOrEmpty(objResult[i].ProjectOwner))
                        {
                            objResult[i].ProjectOwner = "";
                        }
                        if (string.IsNullOrEmpty(objResult[i].Type))
                        {
                            objResult[i].Type = "";
                        }




                        double _percentage, _TotalDaysSpend, _ContractHours = 0;
                        double.TryParse(objResult[i].ActualDuration.ToString(), out _TotalDaysSpend);
                        double.TryParse(objResult[i].ContractHoursPerMonth.ToString(), out _ContractHours);
                        bool _isLimit = false;
                        _TotalDaysSpend = _TotalDaysSpend / 8;// converting into days
                        _ContractHours = _ContractHours / 8; // converting into days
                        _percentage = (_TotalDaysSpend / _ContractHours) * 100;
                        
                        if (Double.IsInfinity(_percentage))
                        {
                            _percentage = (_TotalDaysSpend / _TotalDaysSpend) * 100;
                        }
                        if (Double.IsNaN(_percentage))
                        {
                            _percentage = 0;
                        }
                        if (_percentage >= _thresholdValue)
                        {
                            _isLimit = true;
                        }

                        //if (_ContractHours > 0)
                        //{
                            DashBoardStatsResponse.Add(new DashBoardStatsUpdateBO
                            {
                                ProjectID = Convert.ToInt32(objResult[i].ID),
                                ProjectName = objResult[i].Name.ToString(),
                                TotalDaysSpend = _TotalDaysSpend.ToString("F"),
                                TotalEstimatedDays = _ContractHours.ToString("F"),
                                CompletedPercentage = Math.Round(Convert.ToDouble(_percentage.ToString("F")), 0, MidpointRounding.AwayFromZero),
                                IsLimitExceed = _isLimit,
                                StartDate = objResult[i].StartDate.ToString(),
                                EndDate = objResult[i].EndDate.ToString(),
                                isActive = Convert.ToBoolean(objResult[i].IsActive),
                                ProjectDescription = objResult[i].ProjectDescription.ToString(),
                                ProjectOwner = objResult[i].ProjectOwner.ToString(),
                                ProjectType = objResult[i].Type.ToString(),
                                ClientID = Convert.ToInt32(objResult[i].ClientID),
                                Total_Hours_Spent = Convert.ToDecimal(objResult[i].ActualDuration),
                                ClientName = objResult[i].ClientName,
                            });
                        //}

                    }
                    return DashBoardStatsResponse;
                }
                return null;

            }

            catch (Exception e) { return null; }
        }
        public List<DashBoardStatsBO> GetDashBoardStatsForStaffSearchBL(GetDashBoardStatsForStaffSearchBO objGetState)
        {
            try
            {
                int _StaffID = -1;
                int.TryParse(objGetState.iStaffID, out _StaffID);


                //DateTime dtRequired = DateTime.Now;
                //if (!string.IsNullOrEmpty(objGetState.Year) &&
                //    !string.IsNullOrEmpty(objGetState.Month) &&
                //    objGetState.Year != "-1" &&
                //    objGetState.Month != "-1"
                //   )
                //{
                //    int _year = 0; int _month = 0;
                //    int.TryParse(objGetState.Year, out _year);
                //    int.TryParse(objGetState.Month, out _month);
                //    dtRequired = new DateTime(_year, _month, DateTime.Now.Day);
                //}

                List<DashBoardStatsBO> DashBoardStatsResponse = new List<DashBoardStatsBO>();

                DashboardStatsDAL ObjDAL = new DashboardStatsDAL();

                var objResult = ObjDAL.GetDashboardStatsForStaffSearchDAL (objGetState.StartDate,objGetState.EndDate, objGetState.iStaffID);

                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        double _percentage, _TotalDaysSpend, _TotalProjectHours = 0;
                        double.TryParse(objResult[i].Total_Hours_Spent.ToString(), out _TotalDaysSpend);
                        double.TryParse(objResult[i].TotalProjectHour.ToString(), out _TotalProjectHours);
                        bool _isLimit = false;
                        _TotalDaysSpend = _TotalDaysSpend / 8;// converting into days
                        _TotalProjectHours = _TotalProjectHours / 8; // converting into days
                        _percentage = (_TotalDaysSpend / _TotalProjectHours) * 100;


                        if (_TotalProjectHours > 0)
                        {
                            DashBoardStatsResponse.Add(new DashBoardStatsBO
                            {
                                ProjectID = Convert.ToInt32(objResult[i].ID),
                                ProjectName = objResult[i].Name.ToString(),
                                TotalDaysSpend = _TotalDaysSpend.ToString("F"),
                                TotalEstimatedDays = _TotalProjectHours.ToString("F"),
                                CompletedPercentage = Math.Round(Convert.ToDouble(_percentage.ToString("F")), 0, MidpointRounding.AwayFromZero),
                                IsLimitExceed = false
                            });
                        }

                    }
                    return DashBoardStatsResponse;
                }
                return null;
            }

            catch (Exception e) { return null; }
        }
    }
}


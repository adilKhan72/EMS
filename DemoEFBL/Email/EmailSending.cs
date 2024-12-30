using DemoEFBL.Settings;
using DemoEFBO.Dashboard;
using DemoEFBO.Setting;
using DemoEFDAL.EmailSending;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.Email
{
    public class EmailSending
    {
        #region Dashboard Email Block functions 

        private List<DashBoardStatsBO> GetWeeklyMainDashboardData(double ThresholdValue)
        {
            try
            {
                List<DashBoardStatsBO> Stats = new List<DashBoardStatsBO>();
                EmailDAL objEmailDAL = new EmailDAL();
                var result = objEmailDAL.GetDashboardStatsForEmail(ThresholdValue, null, null);
                if (result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        double _perentage, _TotalDaysSpend, _ContractHours = 0;
                        double.TryParse(result[i].Total_Hours_Spent.ToString(), out _TotalDaysSpend);
                        double.TryParse(result[i].ContractHoursPerMonth.ToString(), out _ContractHours);
                        bool _isLimit = false;
                        _TotalDaysSpend = _TotalDaysSpend / 8;// converting into days
                        _ContractHours = _ContractHours / 8; // converting into days
                        _perentage = (_TotalDaysSpend / _ContractHours) * 100;
                        if (_perentage >= ThresholdValue)
                        {
                            _isLimit = true;
                        }
                        if (_ContractHours > 0 && _isLimit)
                        {
                            Stats.Add(new DashBoardStatsBO
                            {
                                ProjectID = result[i].ID,
                                ProjectName = result[i].Name,
                                TotalDaysSpend = Math.Round(_TotalDaysSpend, 1).ToString(),// .ToString("F"),
                                TotalEstimatedDays = Math.Round(_ContractHours, 1).ToString(),// .ToString("F"),
                                CompletedPercentage = Math.Round(_perentage, 0), //Convert.ToDouble(_perentage.ToString("F")),
                                IsLimitExceed = _isLimit
                            });
                        }
                    }
                    return Stats;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static string getHtmlDashBoard(List<DashBoardStatsBO> lst, string _heading)
        {
            try
            {
                string messageBody = "<font>" + _heading + "</font><br><br>";
                if (lst.Count == 0) return messageBody;
                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style=\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";
                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "Project Name" + htmlTdEnd;
                messageBody += htmlTdStart + "Estimated Days" + htmlTdEnd;
                messageBody += htmlTdStart + "Days Spent" + htmlTdEnd;
                messageBody += htmlTdStart + "Percentage Completed" + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;
                //Loop all the rows from grid vew and added to html td  
                for (int i = 0; i <= lst.Count - 1; i++)
                {
                    messageBody = messageBody + htmlTrStart;
                    messageBody = messageBody + htmlTdStart + lst[i].ProjectName + htmlTdEnd; //adding student name  
                    messageBody = messageBody + htmlTdStart + lst[i].TotalEstimatedDays + htmlTdEnd; //adding DOB  
                    messageBody = messageBody + htmlTdStart + lst[i].TotalDaysSpend + htmlTdEnd; //adding Email  
                    messageBody = messageBody + htmlTdStart + lst[i].CompletedPercentage + htmlTdEnd; //adding Mobile  
                    messageBody = messageBody + htmlTrEnd;
                }
                messageBody = messageBody + htmlTableEnd;
                return messageBody; // return HTML Table as string from this function  
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void SendEmail(List<SettingBO> lstSettings, List<DashBoardStatsBO> lstInfoStats)
        {
            try
            {
                SettingsBAL objSettingBL = new SettingsBAL();
                EmailDAL objEmailDal = new EmailDAL();
                string _Mainheading = "Following Projects have crossed 'Number_of_Days_Per_Month_Contract' limit of '" + objSettingBL.GetSettingValue(lstSettings, "ProjectCompletionThresold") + "'%" + " For the month of " + DateTime.Now.ToString("MMMM") + "," + DateTime.Now.Year;

                string _emailBody = getHtmlDashBoard(lstInfoStats, _Mainheading);
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(objSettingBL.GetSettingValue(lstSettings, "FromEmailAddressForAlert"));    //"rezaidalerts@gmail.com");
                //message.To.Add(new MailAddress("faisal.hameed@rezaid.co.uk"));
                string _toArr = objSettingBL.GetSettingValue(lstSettings, "ToEmailAddressForAlert");
                string[] _Arr = _toArr.Split(',');
                for (int i = 0; i < _Arr.Length; i++)
                {
                    message.To.Add(_Arr[i]);
                }
                message.Subject = objSettingBL.GetSettingValue(lstSettings, "EmailSubject");  //"Test";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = _emailBody;
                message.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
                smtp.Port = Convert.ToInt32(objSettingBL.GetSettingValue(lstSettings, "SMTP-Port"));  //587;
                smtp.Host = objSettingBL.GetSettingValue(lstSettings, "SMTP-Host");   //"smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = Convert.ToBoolean(objSettingBL.GetSettingValue(lstSettings, "SMTP-EnableSSL"));  //true;
                smtp.UseDefaultCredentials = Convert.ToBoolean(objSettingBL.GetSettingValue(lstSettings, "SMTP-UseDefaultCredential"));  //false;
                smtp.Credentials = new NetworkCredential(objSettingBL.GetSettingValue(lstSettings, "FromEmailAddressForAlert"), objSettingBL.GetSettingValue(lstSettings, "FromEmailAddressPassword"));
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                for (int i = 0; i < lstInfoStats.Count; i++)
                {
                    objEmailDal.AddNotification(lstInfoStats[i].ProjectID);
                }
            }
            catch (Exception exx) { exx.ToString(); }
        }

        #endregion
        
        #region Weekly email Block functions

        private void WeeklyEmailCheck(List<SettingBO> lst, double Threshold)
        {
            try
            {
                SettingsBAL objSettngBal = new SettingsBAL();
                EmailDAL objEmailDAL = new EmailDAL();
                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                {
                    string val = objSettngBal.GetSettingValue(lst, "WeeklyEmailCheck");
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (!Convert.ToBoolean(val))
                        {
                            var result = objEmailDAL.GetDashboardStatsForEmail(Threshold,null,null);
                            List<DashBoardStatsBO> List = new List<DashBoardStatsBO>();
                            if (result != null)
                            {
                                for (int i = 0; i < result.Count; i++)
                                {
                                    double _perentage, _TotalDaysSpend, _ContractHours = 0;
                                    double.TryParse(result[i].Total_Hours_Spent.ToString(), out _TotalDaysSpend);
                                    double.TryParse(result[i].ContractHoursPerMonth.ToString(), out _ContractHours);
                                    _TotalDaysSpend = _TotalDaysSpend / 8;// converting into days
                                    _ContractHours = _ContractHours / 8; // converting into days
                                    _perentage = 0;
                                    if (_ContractHours > 0)
                                    {
                                        _perentage = (_TotalDaysSpend / _ContractHours) * 100;
                                        List.Add(new DashBoardStatsBO
                                        {
                                            ProjectID = result[i].ID,
                                            ProjectName = result[i].Name,
                                            TotalDaysSpend = Math.Round(_TotalDaysSpend, 1).ToString(),
                                            TotalEstimatedDays = Math.Round(_ContractHours, 1).ToString(),
                                            CompletedPercentage = Math.Round(_perentage, 0)
                                        });
                                    }
                                }
                            }
                            SendEmailWeekly(lst, List);
                            objEmailDAL.WeeklyEmailValueUpdate("true");
                        }
                    }
                }
                else
                {
                    string val = objSettngBal. GetSettingValue(lst, "WeeklyEmailCheck");
                    if (!string.IsNullOrEmpty(val))
                    {
                        if (Convert.ToBoolean(val))
                        {
                            objEmailDAL.WeeklyEmailValueUpdate("false");
                        }
                    }
                }

            }
            catch (Exception ex) { }
        }

        private void SendEmailWeekly(List<SettingBO> lstSettings, List<DashBoardStatsBO> lstInfoStats)
        {
            try
            {
                SettingsBAL objSettingBAL = new SettingsBAL();
                string _Mainheading = "Weekly Email";
                string _SubHeading = DateTime.Now.ToString("MMMM") + "," + DateTime.Now.Year.ToString() + " (With Management Time)";
                string _emailBody = getHtmlWeekly(lstInfoStats, _Mainheading, _SubHeading);
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(objSettingBAL.GetSettingValue(lstSettings, "FromEmailAddressForAlert"));    //"rezaidalerts@gmail.com");
                string _toArr = objSettingBAL.GetSettingValue(lstSettings, "ToEmailAddressForAlert");
                string[] _Arr = _toArr.Split(',');
                for (int i = 0; i < _Arr.Length; i++)
                {
                    message.To.Add(_Arr[i]);
                }
                message.Subject = "EMS Dashboard [ " + DateTime.Now.ToShortDateString() + " ]"; //GetSettingValue(lstSettings, "EmailSubject");  //"Test";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = _emailBody;
                message.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
                smtp.Port = Convert.ToInt32(objSettingBAL.GetSettingValue(lstSettings, "SMTP-Port"));  //587;
                smtp.Host = objSettingBAL.GetSettingValue(lstSettings, "SMTP-Host");   //"smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = Convert.ToBoolean(objSettingBAL.GetSettingValue(lstSettings, "SMTP-EnableSSL"));  //true;
                smtp.UseDefaultCredentials = Convert.ToBoolean(objSettingBAL.GetSettingValue(lstSettings, "SMTP-UseDefaultCredential"));  //false;
                smtp.Credentials = new NetworkCredential(objSettingBAL.GetSettingValue(lstSettings, "FromEmailAddressForAlert"), objSettingBAL.GetSettingValue(lstSettings, "FromEmailAddressPassword"));
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception exx) { exx.ToString(); }
        }

        private static string getHtmlWeekly(List<DashBoardStatsBO> lst, string _heading, String _SubHeading)
        {
            try
            {
                string messageBody = "<font><b>" + _heading + "</b></font><br>";
                messageBody += "<font>" + _SubHeading + "</font><br><br>";
                if (lst.Count == 0) return messageBody;
                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style=\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";
                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "Projects" + htmlTdEnd;
                messageBody += htmlTdStart + "Spent (Days)" + htmlTdEnd;
                messageBody += htmlTdStart + "Contract (Days)" + htmlTdEnd;
                messageBody += htmlTdStart + "Completed(%)" + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;
                //Loop all the rows from grid vew and added to html td  
                for (int i = 0; i <= lst.Count - 1; i++)
                {
                    messageBody = messageBody + htmlTrStart;
                    messageBody = messageBody + htmlTdStart + lst[i].ProjectName + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + lst[i].TotalDaysSpend + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + lst[i].TotalEstimatedDays + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + lst[i].CompletedPercentage + htmlTdEnd;
                    messageBody = messageBody + htmlTrEnd;
                }
                messageBody = messageBody + htmlTableEnd;
                return messageBody; // return HTML Table as string from this function  
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        public void RunEmailFunctionality()
        {
            try
            {
                SettingsBAL objSettingBL = new SettingsBAL();
                EmailDAL objEmailDal = new EmailDAL();
                var _listSetting = objSettingBL.GetSettings();
                var _response = _listSetting.Find(x => x.vSettingName == "ProjectCompletionThresold");
                double _thresholdValue = 0;
                double.TryParse(_response.vSettingValue, out _thresholdValue);
                WeeklyEmailCheck(_listSetting, _thresholdValue);
                var _listDashboard = GetWeeklyMainDashboardData(_thresholdValue);
                var _DashboardList = new List<DashBoardStatsBO>();
                if (_listDashboard != null && _listDashboard.Count > 0)
                {
                    for (int i = 0; i < _listDashboard.Count; i++)
                    {
                        int _projectID = _listDashboard[i].ProjectID;
                        if (_projectID > 0)
                        {
                            bool result = objEmailDal.IsEmailAlreadySent(_projectID, DateTime.Now);
                            if (!result)
                            {
                                _DashboardList.Add(_listDashboard[i]);
                            }
                        }
                    }
                }
                if (_DashboardList != null && _DashboardList.Count > 0)
                {
                    SendEmail(_listSetting, _DashboardList);
                }
            }
            catch (Exception ex)
            {

            }
        }





    }
}

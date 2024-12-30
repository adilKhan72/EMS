using DemoEFBL.Settings;
using DemoEFBO.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.ForgetPassword;
using DemoEFDAL.ForgetPassword;
using DemoEFCommon.Setting;

namespace DemoEFBL.ForgetPassword
{
    public class ForgetPasswordBL
    {
    

        public ForgetPasswordBO ForgetPasswordLogic(string Useremail)
        {

            try
            {
                ForgetPasswordBO ForgetPasswordResponse = new ForgetPasswordBO();
                ForgetPasswordDAL ObjDAL = new ForgetPasswordDAL();

                var objResult = ObjDAL.ForgetPassword(Useremail);
                if (objResult != null)
                {

                    ForgetPasswordResponse.UserID = Convert.ToInt32(objResult.TaskOwnerID);
                    ForgetPasswordResponse.LoginID = Convert.ToInt32(objResult.UserProfileTableID);
                    ForgetPasswordResponse.GlobalID = objResult.GlobalID;
                    ForgetPasswordResponse.Email = objResult.EmailAddress;

                    SettingsBAL objSettingBL = new SettingsBAL();
                    SettingsBAL objSettingBAL = new SettingsBAL();
                    List<SettingBO> lstSettings = objSettingBL.GetSettings();

                    string subject = "Reset Password-EMS";
                    string emailTo = ForgetPasswordResponse.Email;
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient();
                    mail.From = new MailAddress(objSettingBAL.GetSettingValue(lstSettings, "FromEmailAddressForAlert"));
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    //mail.Body = body;
                    mail.IsBodyHtml = true;
                    string url = HttpUtility.HtmlEncode("https://rezaidems.co.uk/ResetPassword?GI=" + ForgetPasswordResponse.GlobalID + "&LI=" + ForgetPasswordResponse.LoginID + "&UI=" + ForgetPasswordResponse.UserID);
                    //string url = HttpUtility.HtmlEncode(Setting.forget_password_link + "ResetPassword?GI=" + ForgetPasswordResponse.GlobalID + "&LI=" + ForgetPasswordResponse.LoginID + "&UI=" + ForgetPasswordResponse.UserID);
                    mail.Body = "<html><body><h2>RESET PASSWORD</h2></br><a href=\"" + url + "\">Click Here to Redirect to Reset Password Screen</a></body></html>";
                    SmtpServer.Host = objSettingBAL.GetSettingValue(lstSettings, "SMTP-Host");  
                    SmtpServer.Port = Convert.ToInt32(objSettingBAL.GetSettingValue(lstSettings, "SMTP-Port"));
                    SmtpServer.UseDefaultCredentials = Convert.ToBoolean(objSettingBAL.GetSettingValue(lstSettings, "SMTP-UseDefaultCredential"));
                    SmtpServer.EnableSsl = Convert.ToBoolean(objSettingBAL.GetSettingValue(lstSettings, "SMTP-EnableSSL"));

                    SmtpServer.Credentials = new NetworkCredential(objSettingBAL.GetSettingValue(lstSettings, "FromEmailAddressForAlert"), objSettingBAL.GetSettingValue(lstSettings, "FromEmailAddressPassword"));
                    SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;


                    SmtpServer.Send(mail);


                    return ForgetPasswordResponse;
                }
                return null;
            }
            catch (Exception e) { return null; }
        }
    }
}

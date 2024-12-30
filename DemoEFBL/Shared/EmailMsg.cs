using DemoEFBL.Settings;
using DemoEFBO.Setting;
using DemoEFCommon.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DemoEFBL.Shared
{
    public class EmailMsg
    {
        public void EmailSendingFun(int UserID, string EmailTo,string EmailTitle, string EmailMsg)
        {

            try
            {
             

                    SettingsBAL objSettingBL = new SettingsBAL();
                    SettingsBAL objSettingBAL = new SettingsBAL();
                    List<SettingBO> lstSettings = objSettingBL.GetSettings();

                    string subject = EmailTitle;
                    string emailTo = EmailTo;
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient();
                    mail.From = new MailAddress(objSettingBAL.GetSettingValue(lstSettings, "FromEmailAddressForAlert"));
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    //mail.Body = "<html><body><h2>\"" + EmailTitle + "\"</h2></br>\"" + EmailMsg + "\"</body>";

                    string url = HttpUtility.HtmlEncode(Setting.forget_password_link + "ResetPassword?LI=" + UserID + "&WarningMsg="+1);
                    mail.Body = "<html><body><h2>\"" + EmailTitle + "\"</h2></br>\"" + EmailMsg + "\"</body>" +
                    "</br></br></br>if it wasn't you click the link below otherwise ignore this mail</br><a href=\"" + url + "\">Click Here to Redirect to Reset Password Screen</a></html>";
                    SmtpServer.Host = objSettingBAL.GetSettingValue(lstSettings, "SMTP-Host");
                    SmtpServer.Port = Convert.ToInt32(objSettingBAL.GetSettingValue(lstSettings, "SMTP-Port"));
                    SmtpServer.UseDefaultCredentials = Convert.ToBoolean(objSettingBAL.GetSettingValue(lstSettings, "SMTP-UseDefaultCredential"));
                    SmtpServer.EnableSsl = Convert.ToBoolean(objSettingBAL.GetSettingValue(lstSettings, "SMTP-EnableSSL"));

                    SmtpServer.Credentials = new NetworkCredential(objSettingBAL.GetSettingValue(lstSettings, "FromEmailAddressForAlert"), objSettingBAL.GetSettingValue(lstSettings, "FromEmailAddressPassword"));
                    SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;


                    SmtpServer.Send(mail);


                 
                
                
            }
            catch (Exception e) { throw e;  }
        }
    }
}


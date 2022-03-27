using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SMS.App.Utilities.EmailServices
{
    public class EmailService
    {
        static string FromEmail = "nobleschoolbd@gmail.com";
        //static string FromEmailPassword = "123AsD,./";
        static string FromEmailPassword = "qaycmjyyxgrgtvqa";
        static string EmailBody = "";
        public static bool SendEmail(string toEmail, string emailSubject, string mailBody, List<IFormFile> fileToAttach)
        {
            EmailBody = "<!DOCTYPE html><html><body>" + mailBody + "</body></html>";
            #region Mail Message
            MailMessage mail = new MailMessage(
                new MailAddress(FromEmail, emailSubject),
                new MailAddress(toEmail)
                );
            #endregion

            #region Mail Content
            mail.Subject = emailSubject;
            mail.Body = EmailBody;
            mail.IsBodyHtml = true;
            #endregion

            #region Mail Attachment
            if (fileToAttach != null)
            {
                foreach (var file in fileToAttach)
                {
                    mail.Attachments.Add(new Attachment(file.OpenReadStream(), file.FileName));
                }
            }
            #endregion           

            #region Mail Credentials
            NetworkCredential credential = new NetworkCredential();
            credential.UserName = FromEmail;
            credential.Password = FromEmailPassword;
            #endregion

            #region Mail Server Details
            SmtpClient smtp = new SmtpClient();
            if (toEmail.Substring(toEmail.Length - 9) == "gmail.com")
            {
                smtp.Host = "smtp.gmail.com";                
            }
            else if (toEmail.Substring(toEmail.Length - 11) == "outlook.com" || toEmail.Substring(toEmail.Length - 11) == "hotmail.com")
            {
                smtp.Host = "smtp.office365.com";
            }
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = credential;
            smtp.UseDefaultCredentials = false;
            #endregion

            smtp.Send(mail);

            return true;
        }
        public static bool SendEmail(string toEmail, string emailSubject, string mailBody)
        {
            EmailBody = "<!DOCTYPE html><html><body>" + mailBody + "</body></html>";
            #region Mail Message
            MailMessage mail = new MailMessage(
                new MailAddress(FromEmail, emailSubject),
                new MailAddress(toEmail)
                );
            #endregion

            #region Mail Content
            mail.Subject = emailSubject;
            mail.Body = EmailBody;
            mail.IsBodyHtml = true;
            #endregion

            #region Mail Server Details
            SmtpClient smtp = new SmtpClient();
            if (toEmail.Substring(toEmail.Length - 9) == "gmail.com")
            {
                smtp.Host = "smtp.gmail.com";
            }
            else if (toEmail.Substring(toEmail.Length - 11) == "outlook.com" || toEmail.Substring(toEmail.Length - 11) == "hotmail.com")
            {
                smtp.Host = "smtp.office365.com";
            }
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;            
            #endregion

            #region Mail Credentials
            NetworkCredential credential = new NetworkCredential();
            credential.UserName = FromEmail;
            credential.Password = FromEmailPassword;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = credential;
            #endregion


            try
            {
                smtp.Send(mail);
                return true;
            }
            catch (System.Exception)
            {

                return false;
                
            }
        }
    }
}

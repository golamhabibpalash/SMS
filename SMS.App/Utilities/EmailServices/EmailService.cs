using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SMS.App.Utilities.EmailServices
{
    public class EmailService
    {
        static int boysCount = 0;
        static string FromEmail = "nobleschoolbd@gmail.com";
        //static string FromEmailPassword = "123AsD,./";
        static string FromEmailPassword = "qaycmjyyxgrgtvqa";
        static string EmailBody = "";
        static string EmailTemplate = $@"<!DOCTYPE html>
<html>

<head>
  <meta charset=""UTF-8"">
  <title>Attendance Summary(10/07/2023)</title>
  <style>
    body {{
      font-family: Arial, sans-serif;
      color: #333333;
      min-width: 400px;
      max-width: 600px;
      margin:0 auto;
      border:1px solid navy;
      margin-top:10px;
    }}
    
    .header {{
      background-color: #F4F4F4;
      text-align: center;
    }}
    .header h1{{
      background-color: navy;
      color: #fff;
      padding: 10px;
      margin-top: 0px;
    }}
    .content {{
      padding: 20px;
      padding-top: 0;
    }}
    
    .summary-table {{
      width: 100%;
      border-collapse: collapse;
    }}
    
    .summary-table th,
    .summary-table td {{
      padding: 8px;
      text-align: left;
      border-bottom: 1px solid #CCCCCC;
    }}
    
    .summary-table th {{
      background-color: #F4F4F4;
      font-weight: bold;
    }}
  </style>
</head>

<body>
  <div class=""header"">
    <h1>Noble Residential High Schoool</h1>
    <h3>Attendance Summary</h3>
    <h4>Date : 10 July 2023</h4>
  </div>

  <div class=""content"">
    <h2>Dear Sir/Madam,</h2>

    <p>Here is the attendance summary for students and teachers:</p>

    <h3>Student Attendance Summary</h3>
    <table class=""summary-table"">
      <tr>
        <th>Student Type</th>
        <th>Attendance Count</th>
      </tr>
      <!-- Student attendance data goes here -->
      <tr>
        <td>Boys Student</td>
        <td>142</td>
      </tr>
      <tr>
        <td>Girls Student</td>
        <td>93</td>
      </tr>
    </table>

    <h3>Teacher Attendance Summary</h3>
    <table class=""summary-table"">
      <tr>
        <th>Teacher Type</th>
        <th>Attendance Count</th>
      </tr>
      <!-- Teacher attendance data goes here -->
      <tr>
        <td>All Teacher</td>
        <td>"+boysCount+$@"</td>
      </tr>
    </table>

    <p>Thank you for your attention.</p>

    <p>Best regards,<br> Noble Residential School</p>
  </div>
</body>

</html>
";
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

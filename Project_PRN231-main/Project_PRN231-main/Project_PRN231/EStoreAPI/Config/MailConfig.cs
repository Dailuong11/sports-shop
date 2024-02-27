using BusinessObject.Models;
using BusinessObject.Res;
using DocumentFormat.OpenXml.Spreadsheet;
using EStoreAPI.Template;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace EStoreAPI.Config
{
    public class MailConfig
    {
        public static bool SendRecoveryMail(string email, string password, IConfiguration configuration)
        {
            bool isSend = false;
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    SmtpClient SmtpServer = new SmtpClient();
                    mail.From = new MailAddress(configuration.GetValue<string>("Smtp:FromAddress"));
                    mail.To.Add(email);
                    mail.Subject = "Password Recovery";
                    mail.Body = string
                        .Format(EmailTemplate.RECOVERY_EMAIL_TEMPLATE, email, password);
                    mail.IsBodyHtml = true;
                    SmtpServer.UseDefaultCredentials = false;
                    NetworkCredential NetworkCred = new NetworkCredential(
                        configuration.GetValue<string>("Smtp:UserName"),
                        configuration.GetValue<string>("Smtp:Password")
                        );
                    SmtpServer.Credentials = NetworkCred;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Port = configuration.GetValue<int>("Smtp:Port");
                    SmtpServer.Host = configuration.GetValue<string>("Smtp:Server");
                    SmtpServer.Send(mail);
                    isSend = true;
                }
            }
            catch (SmtpException ex)
            {
                string msg = ex.Message;
            }
            return isSend;
        }

        public static bool SendMail(string name ,string email, string password, IConfiguration configuration)
        {
            bool isSend = false;
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    SmtpClient SmtpServer = new SmtpClient();
                    mail.From = new MailAddress(configuration.GetValue<string>("Smtp:FromAddress"));
                    mail.To.Add(email);
                    mail.Subject = "Employee account information";
                    mail.Body = string
                        .Format(EmailTemplate.EMAIL_TEMPLATE, name, email, password);
                    mail.IsBodyHtml = true;
                    SmtpServer.UseDefaultCredentials = false;
                    NetworkCredential NetworkCred = new NetworkCredential(
                        configuration.GetValue<string>("Smtp:UserName"),
                        configuration.GetValue<string>("Smtp:Password")
                        );
                    SmtpServer.Credentials = NetworkCred;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Port = configuration.GetValue<int>("Smtp:Port");
                    SmtpServer.Host = configuration.GetValue<string>("Smtp:Server");
                    SmtpServer.Send(mail);
                    isSend = true;
                }
            }
            catch (SmtpException ex)
            {
                string msg = ex.Message;
            }
            return isSend;
        }

        public static bool SendOrderMail(OrderRes order ,string email, IConfiguration configuration)
        {
            bool isSend = false;
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    SmtpClient SmtpServer = new SmtpClient();
                    mail.From = new MailAddress(configuration.GetValue<string>("Smtp:FromAddress"));
                    mail.To.Add(email);
                    mail.Subject = "Order Confirmation";
                    mail.IsBodyHtml = true;
                    mail.AlternateViews.Add(GetEmbeddedImage(configuration.GetValue<string>("Logo:Path"), order, email));
                    SmtpServer.UseDefaultCredentials = false;
                    NetworkCredential NetworkCred = new NetworkCredential(
                        configuration.GetValue<string>("Smtp:UserName"),
                        configuration.GetValue<string>("Smtp:Password")
                        );
                    SmtpServer.Credentials = NetworkCred;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Port = configuration.GetValue<int>("Smtp:Port");
                    SmtpServer.Host = configuration.GetValue<string>("Smtp:Server");
                    SmtpServer.Send(mail);
                    isSend = true;
                }
            }
            catch (SmtpException ex)
            {
                string msg = ex.Message;
            }
            return isSend;
        }

        private static AlternateView GetEmbeddedImage(String filePath, OrderRes order, string email)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = EmailTemplate.OrderInvoiceTemplate(order, email, res.ContentId);
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

    }
}

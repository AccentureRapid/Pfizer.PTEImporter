using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Net.Mail.Smtp;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Pfizer.PTEImporter.Services
{
    public class EmailService : IEmailService
    {
        private IEventBus _eventBus;
        private readonly ISmtpEmailSender _emailSender;
        private string _host;
        private int _port;
        public EmailService(
            ISmtpEmailSender emailSender,
            IEventBus eventBus)
        {
            _emailSender = emailSender;
            _eventBus = eventBus;

            InitializeSMTP();
        }
        public void SendEmail(string from, string recipients, string subject, string body, object userToken = null)
        {
            var client = _emailSender.BuildClient();

            client.Host = _host;
            client.Port = _port;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.EnableSsl = false;
            client.UseDefaultCredentials = true;

            client.SendAsync(
            from: from,
            recipients: recipients,
            subject: subject,
            body: body,
            userToken: userToken
            );
        }

        public void SendEmail(string from, string recipients, string subject, string body, string[] files)
        {
            var client = _emailSender.BuildClient();

            client.Host = _host;
            client.Port = _port;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.EnableSsl = false;
            client.UseDefaultCredentials = true;

            using (var mail = new MailMessage(from, recipients))
            {
                if (files.Any())
                {
                    foreach (var item in files)
                    {
                        var attach = new Attachment(item);
                        mail.Attachments.Add(attach);
                    }
                }

                mail.Subject = subject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Body = body;
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                client.Send(mail);
            }
        }


        public void SendEmail(string from, string[] recipients, string subject, string body, string[] files)
        {
            var client = _emailSender.BuildClient();

            client.Host = _host;
            client.Port = _port;
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.EnableSsl = false;
            client.UseDefaultCredentials = true;

            MailAddress fromAddress = new MailAddress(from);
            using (var mail = new MailMessage())
            {
                if (files.Any())
                {
                    foreach (var item in files)
                    {
                        var attach = new Attachment(item);
                        mail.Attachments.Add(attach);
                    }
                }

                mail.From = fromAddress;
                mail.Subject = subject;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Body = body;
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;
                if (recipients.Any())
                {
                    foreach (var item in recipients)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            mail.To.Add(item);
                        }
                    }
                }


                client.Send(mail);
            }
        }
        [UnitOfWork]
        private async void InitializeSMTP()
        {
            var host = ConfigurationManager.AppSettings["Host"];
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("Host not configured.");

            var port = ConfigurationManager.AppSettings["Port"];
            if (string.IsNullOrEmpty(port))
                throw new ArgumentNullException("Port not configured.");

            _host = host;
            _port = Convert.ToInt32(port);
        }
    }
}

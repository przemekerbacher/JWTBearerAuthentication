using JWTAuthentication.Models;
using JWTAuthentication.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JWTAuthentication.Helpers
{
    public class MailSender : IMailSender
    {
        private AppSettings _appSettings;
        private SmtpClient _smtpClient;

        public MailSender(AppSettings appSettings, SmtpClient smtpClient)
        {
            _appSettings = appSettings;
            _smtpClient = smtpClient;
        }
        public void Send(MailModel model)
        {
            MailMessage message = new MailMessage();
            message.To.Add(model.To);
            message.Subject = model.Subject;
            message.Body = model.PlainTextContent;
            message.From = new MailAddress(model.From);
            message.IsBodyHtml = false;

            _smtpClient.Send(message);
        }
    }
}

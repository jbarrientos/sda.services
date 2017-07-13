using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;

/*
 * Author: Jorge Barrientos
 * Description: Class to send emails
 */

namespace SDA.Services.Helpers
{
    public class Email
    {
        // constants
        private const string HtmlEmailHeader = "<html><head><title></title></head><body style='font-family:arial; font-size:14px;'>";
        private const string HtmlEmailFooter = "</body></html>";

        // properties
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public List<Attachment> Attachments { get; set; }

        // constructor
        public Email()
        {
            To = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
            Attachments = new List<Attachment>();
        }

        // send

      
        public void Send()
        {
            MailMessage message = new MailMessage();
            
            foreach (var x in To)
            {
                message.To.Add(x);
            }
            foreach (var x in CC)
            {
                message.CC.Add(x);
            }
            foreach (var x in BCC)
            {
                message.Bcc.Add(x);
            }

            message.Subject = Subject;
            message.Body = string.Concat(HtmlEmailHeader, Body, HtmlEmailFooter);
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.From = new MailAddress(From);
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            foreach(var attach in this.Attachments)
            {
                message.Attachments.Add(attach);
            }
            //
            SmtpClient client = new SmtpClient(
                HelperConstant.EMAIL_SERVER, 
                HelperConstant.EMAIL_PORT);
            System.Net.NetworkCredential rs = new System.Net.NetworkCredential(
                HelperConstant.EMAIL_DEFAULT_ACCOUNT, 
                HelperConstant.EMAIL_ACCOUNT_PASSWORD);

            client.Credentials = rs;
            // Llamada Asincrona
            new Thread(() => { client.Send(message); }).Start();

            //client.Send(message);
        }
    }
}

using AaaS.Dal.Ado.Attributes;
using AaaS.Domain;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Core.Actions
{
    public class MailAction : BaseAction
    {
        public string MailAddress { get; set; }

        public string MailContent { get; set; }

        [Volatile]
        public SendGridClient SendGridClient { set; protected get; }

        public MailAction(SendGridClient sendGridClient)
        {
            SendGridClient = sendGridClient;
        }

        public MailAction() {
            new Task(async () =>
            {
                await Task.Delay(10000);
                if (SendGridClient is null)
                {
                    //throw new ArgumentException("SendGridClient is Null after 4s");
                }
            }).Start();
        }

        public async override Task Execute()
        {
            //await SendMailFromTemplate("d-a56ee3e37dce4ec58b51545ea2107d81", new { mailContent = MailContent }, MailAddress);
            Console.WriteLine($"{MailContent.Take(30)}... sent to {MailAddress}"); // TODO: Mail tatsächlich abschicken lassen
        }

        public async Task<SendGrid.Response> SendMailFromTemplate(string templateID, object templateData, string recipientMail, string recipientName = null)
        {
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom("s2010307058@students.fh-hagenberg.at", "AaaS");
            sendGridMessage.AddTo(recipientMail, recipientName);
            sendGridMessage.SetTemplateId(templateID);
            sendGridMessage.SetTemplateData(templateData);
            var response = await SendGridClient.SendEmailAsync(sendGridMessage);
            return response;
        }

    }
}

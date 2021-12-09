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

        private readonly SendGridClient sendGridClient;

        public MailAction()
        {
            sendGridClient = new SendGridClient("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");
        }

        public async override Task Execute()
        {
            await SendMailFromTemplate("d-a56ee3e37dce4ec58b51545ea2107d81", new { mailContent = MailContent }, MailAddress);
        }

        public async Task<SendGrid.Response> SendMailFromTemplate(string templateID, object templateData, string recipientMail, string recipientName = null)
        {
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom("s2010307058@students.fh-hagenberg.at", "AaaS");
            sendGridMessage.AddTo(recipientMail, recipientName);
            sendGridMessage.SetTemplateId(templateID);
            sendGridMessage.SetTemplateData(templateData);
            var response = await sendGridClient.SendEmailAsync(sendGridMessage);
            return response;
        }

    }
}

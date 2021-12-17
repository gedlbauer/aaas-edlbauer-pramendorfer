using AaaS.Core.Actions;
using Microsoft.Extensions.Hosting;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AaaS.Core.HostedServices
{
    public class HeartbeatService : BackgroundService
    {
        private readonly SendGridClient _sendGrid;
        private Dictionary<Guid, DateTime> Heartbeats { get; } = new();

        public HeartbeatService(SendGridClient sendGridClient)
        {
            _sendGrid = sendGridClient;
        }

        public void AddHeartbeat(Guid creatorId)
        {
            Heartbeats[creatorId] = DateTime.UtcNow;
        }

        public void LogOffClient(Guid creatorId)
        {
            if (!Heartbeats.ContainsKey(creatorId))
                throw new ArgumentException($"{creatorId} not registerd.");

            Heartbeats.Remove(creatorId);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    CheckHeartbeats();
                    await Task.Delay(30_000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        private void CheckHeartbeats()
        {
            foreach (var hearbeat in Heartbeats)
            {
         
                if((DateTime.UtcNow - hearbeat.Value).TotalSeconds > 30)
                {
                    Heartbeats.Remove(hearbeat.Key);
                }
            }
        }

        private async Task SendWarningEmail()
        {
            //var sendGridMessage = new SendGridMessage();
            //sendGridMessage.SetFrom("s2010307058@students.fh-hagenberg.at", "AaaS");
            //sendGridMessage.AddTo(recipientMail, recipientName);
            //sendGridMessage.SetTemplateId(templateID);
            //sendGridMessage.SetTemplateData(templateData);
            //var response = await _sendGridClient.SendEmailAsync(sendGridMessage);
            //await SendMailFromTemplate("d-a56ee3e37dce4ec58b51545ea2107d81", new { mailContent = MailContent }, MailAddress);
        }
    }
}

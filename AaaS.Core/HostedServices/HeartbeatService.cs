using AaaS.Core.Actions;
using AaaS.Core.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
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
        private readonly ISendGridClient _sendGrid;
        private readonly HeartbeatOptions _options;
        private Dictionary<Guid, DateTime> Heartbeats { get; } = new();

        public HeartbeatService(ISendGridClient sendGridClient, IOptions<HeartbeatOptions> options)
        {
            _sendGrid = sendGridClient;
            _options = options.Value;
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
                    await CheckHeartbeats();
                    await Task.Delay(_options.Interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        private async Task CheckHeartbeats()
        {
            foreach (var hearbeat in Heartbeats)
            {
                if((DateTime.UtcNow - hearbeat.Value).TotalMilliseconds > _options.Threshold)
                {
                    await SendWarningEmail(hearbeat.Key);
                    Heartbeats.Remove(hearbeat.Key);
                }
            }
        }

        private async Task SendWarningEmail(Guid creatorId)
        {
            var message = new SendGridMessage();
            message.SetFrom("s2010307058@students.fh-hagenberg.at", "AaaS");
            message.SetSubject("Client instance anomaly");
            message.AddContent("text/plain", $"Client instance ${creatorId} did log off, but stopped sending hearbeats!");
            message.AddTo(_options.WarningEMailAddress, "Admin");

            await _sendGrid.SendEmailAsync(message);
        }
    }
}

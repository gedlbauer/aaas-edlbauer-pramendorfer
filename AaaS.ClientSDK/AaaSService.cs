using AaaS.ClientSDK.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.ClientSDK
{
    public class AaaSService
    {
        private const string baseUrl = "https://localhost:5001";
        private static AaaSService instance;
        private readonly AaaSServiceOptions _options;
        private readonly HttpClient _httpClient;
        private readonly Client _aaasClient;
        private readonly Dictionary<string, TimeMeasurementInsertDto> _timeMeasurements = new();

        private bool IsRunning { get; set; }

        public static AaaSService Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("AaaSService not created");
                }
                return instance;
            }
        }
        public static void Create(HttpClient client, AaaSServiceOptions options)
        {
            if (instance != null)
            {
                throw new Exception("AaaSService already created");
            }
            instance = new AaaSService(client, options);

        }

        private AaaSService(HttpClient client, AaaSServiceOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpClient = client ?? throw new ArgumentNullException(nameof(client));
            SetupHttpClient();
            _aaasClient = new Client(baseUrl, _httpClient);
            IsRunning = true;
            _ = StartHeartbeating();
        }

        private void SetupHttpClient()
        {
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _options.ApiKey);
        }

        private async Task StartHeartbeating()
        {
            while(IsRunning)
            {
                await SendHeartbeat();
                await Task.Delay(5000);
            }
        }

        public async Task Stop()
        {
            IsRunning = false;
            await _aaasClient.LogoffAsync(_options.CreatorId);
        }
          
        
        private async Task SendHeartbeat()
            => await _aaasClient.HeartbeatCommandAsync(_options.CreatorId);

        public async Task<IEnumerable<LogType>> GetLogTypes()
            => await _aaasClient.TypesAsync();

        public async Task InsertLog(LogInsertDto log)
            => await _aaasClient.LogsAsync(log);
        
        public async Task InsertTimeMeasurement(TimeMeasurementInsertDto timeMeasurement)
            => await _aaasClient.TimeMeasurementsAsync(timeMeasurement);
        
        public async Task InsertCounter(CounterInsertDto counter)
            => await _aaasClient.CountersAsync(counter);
        
        public async Task InsertMeasurement(MeasurementInsertDto measurement)
            => await _aaasClient.MeasurementsAsync(measurement);

        public void StartTiming(string name, Guid creatorId)
        {
            if(!_timeMeasurements.ContainsKey(name))
            {
                _timeMeasurements[name] = new TimeMeasurementInsertDto
                {
                    CreatorId = creatorId,
                    StartTime = DateTime.Now,
                    Name = name
                };
            } else
            {
                throw new InvalidOperationException("Time Measurement already started.");
            }
        }

        public async Task StopAndInsertTiming(string name)
        {
            if (!_timeMeasurements.ContainsKey(name)) throw new ArgumentException($"No Time Measurement with name {name} started.");

            _timeMeasurements[name].EndTime = DateTime.Now;
            _timeMeasurements[name].Timestamp = DateTime.Now;
            await InsertTimeMeasurement(_timeMeasurements[name]);
            _timeMeasurements.Remove(name);
        }
        
    }
}

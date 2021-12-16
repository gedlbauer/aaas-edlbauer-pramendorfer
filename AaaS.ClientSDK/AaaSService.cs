﻿using AaaS.ClientSDK.Options;
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
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly Client _aaasClient;
        public AaaSService(HttpClient client, AaaSServiceOptions options)
        {
            _apiKey = options?.ApiKey ?? throw new ArgumentNullException(nameof(options));
            _httpClient = client ?? throw new ArgumentNullException(nameof(client));
            SetupHttpClient();
            _aaasClient = new Client(baseUrl, _httpClient);
        }

        private void SetupHttpClient()
        {
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
        }

        public async Task InsertLog(LogInsertDto log)
            => await _aaasClient.LogsAsync(log);
        

        public async Task InsertTimeMeasurement(TimeMeasurementInsertDto timeMeasurement)
            => await _aaasClient.TimeMeasurementsAsync(timeMeasurement);
        
        public async Task InsertCounter(CounterInsertDto counter)
            => await _aaasClient.CountersAsync(counter);
        
        public async Task InsertMeasurement(MeasurementInsertDto measurement)
            => await _aaasClient.MeasurementsAsync(measurement);
        
    }
}
